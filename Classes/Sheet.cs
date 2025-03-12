using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace vlc_works
{
    public class Sheet
    {
        private SheetsService SheetsService { get; set; }

        private string SheetId { get; set; }
        private string ListName { get; set; } = null;

        public Sheet(string sheetId, string listName)
        {
            GoogleCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read)) {
                credential = GoogleCredential.FromStream(stream)
                   .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            SheetsService = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "GoldInSafe",
            });

            SheetId = sheetId;
            ListName = listName;
        }

        private string Range(ListAndRange listAndRange) =>
            listAndRange.List == null
            ? listAndRange.FromList(ListName)
            : listAndRange.ToString();

        public string[][] Get(ListAndRange listAndRange)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                SheetsService
                .Spreadsheets
                .Values
                .Get(SheetId, Range(listAndRange));

            var response = request.Execute();

            return
                response
                ?.Values
                .Select(row => row.Select(cell => cell.ToString()).ToArray())
                .ToArray();
        }

        public void Append(ListAndRange listAndRange, string[][] values)
        {
            string range = Range(listAndRange);

            ValueRange body = new ValueRange {
                Range = range,
                Values = values
            };

            var request =
                SheetsService
                .Spreadsheets
                .Values
                .Append(body, SheetId, range);
            request.ValueInputOption =
                SpreadsheetsResource
                .ValuesResource
                .AppendRequest
                .ValueInputOptionEnum
                .USERENTERED;

            request.Execute();
        }
    }
}
