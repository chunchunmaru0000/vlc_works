using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace vlc_works
{
    public class Sheets
    {
        private string ApiKey { get; set; }
        private string SheetId { get; set; }
        private HttpClient Client = new HttpClient();

        public Sheets(string apiKey, string sheetId)
        {
            ApiKey = apiKey;
            SheetId = sheetId;
        }

        private string SheetsUrl(SheetAndRange sheetAndRange) =>
            $"https://sheets.googleapis.com/v4/spreadsheets/{SheetId}/values/{sheetAndRange}?key={ApiKey}";

        private class GoogleSheetsGetResponse
        {
            [JsonProperty("range")]
            public string Range { get; set; }
            [JsonProperty("values")]
            public string[][] Values { get; set; }
        }

        public async Task<string[][]> Get(SheetAndRange sheetAndRange)
        {
            string url = SheetsUrl(sheetAndRange);
            HttpResponseMessage response = await Client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            try {
                Task<string> jsonTask = response.Content.ReadAsStringAsync();
                return
                    JsonConvert
                    .DeserializeObject<GoogleSheetsGetResponse>(jsonTask.Result)
                    .Values;
            } catch {
                return null;
            }
        }
    }
}
