using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace vlc_works
{
    public class Sheets
    {
        private SheetsService SheetsService { get; set; }

        private string SheetId { get; set; }

        public Sheets(string apiKey, string sheetId)
        {
            GoogleCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read)) {
                credential = GoogleCredential.FromStream(stream)
                   .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            SheetsService = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "safe",
            });

            SheetId = sheetId;
        }

        public string[][] Get(SheetAndRange sheetAndRange)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                SheetsService
                .Spreadsheets
                .Values
                .Get(SheetId, sheetAndRange.ToString());

            var response = request.Execute();

            return
                response
                ?.Values
                .Select(row => row.Select(cell => cell.ToString()).ToArray())
                .ToArray();
        }

        public void Put(SheetAndRange sheetAndRange, string[][] values)
        {
            ValueRange body = new ValueRange {
                Range = sheetAndRange.ToString(),
                Values = values
            };

            var request =
                SheetsService
                .Spreadsheets
                .Values
                .Update(body, SheetId, sheetAndRange.ToString());
            request.ValueInputOption =
                SpreadsheetsResource
                .ValuesResource
                .UpdateRequest
                .ValueInputOptionEnum
                .RAW;

            request.Execute();
        }
    }
}
