using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data = Google.Apis.Sheets.v4.Data;

namespace PushValueToGoogleSheet
{
    public class PushValueToGoogleSheetHelper
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private const string GoogleCredentialsFileName = "credentials.json";
        public int number = 0;
        public List<string> list = new List<string>();

        private static SheetsService GetSheetsService()
        {
            using (var stream = new FileStream(GoogleCredentialsFileName, FileMode.Open, FileAccess.Read))
            {
                var serviceInitializer = new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromStream(stream).CreateScoped(Scopes)
                };
                return new SheetsService(serviceInitializer);
            }
        }
        public void PushValueToGoogle(int rawGoogleSheet, string sheetId, string filePath, string range)
        {
            var serviceValues = GetSheetsService().Spreadsheets.Values;
            string[] songLog = System.IO.File.ReadAllLines(filePath);
            if (songLog.Length == 0)
            {
                string messageError = $"Không có dữ liệu để đẩy lên Google Sheet";
                string caption = "Error";
                var result = MessageBox.Show(messageError, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    return;
                }
            }
            string valueInputOption = "USER_ENTERED";  // TODO: Update placeholder value.
            // The new values to apply to the spreadsheet.
            List<Data.ValueRange> data = new List<Data.ValueRange>();  // TODO: Update placeholder value.
            ValueRange values = new ValueRange();
            string sheetNameTrackingSong = range;
            values.Range = $"{sheetNameTrackingSong}!A{rawGoogleSheet}";
            List<IList<object>> temp = new List<IList<object>>();
            for (int index = 0; index < songLog.Length; index++)
            {
                try
                {
                    List<object> row = new List<object> { };
                    string[] songLineArr = songLog[index].Split(new[] { "|" }, StringSplitOptions.None);
                    for (int indexString = 0; indexString < songLineArr.Length; indexString++)
                    {

                        string valuess = songLineArr[indexString];
                        row.Add(valuess);
                    }
                    List<IList<object>> temp2 = new List<IList<object>> { row };
                    temp.AddRange(temp2);
                }
                catch
                {

                }
            }
            values.Values = temp;
            data.Add(values);

            // TODO: Assign values to desired properties of `requestBody`:
            Data.BatchUpdateValuesRequest requestBody = new Data.BatchUpdateValuesRequest();
            requestBody.ValueInputOption = valueInputOption;
            requestBody.Data = data;
            SpreadsheetsResource.ValuesResource.BatchUpdateRequest request = GetSheetsService().Spreadsheets.Values.BatchUpdate(requestBody, sheetId);
            Data.BatchUpdateValuesResponse response = request.Execute();
            Console.WriteLine(JsonConvert.SerializeObject(response));
        }
        public void getGoogleSheet(string spreadsheetId, string range)
        {
            string spreadsheetIds = "1xGW0ZGyCPY3SPQpZ6aI_i3SlYYHAIZvwMP5dw8R37W0";  // TODO: Update placeholder value.

            // The A1 notation of the values to retrieve.
            List<string> ranges = new List<string>();  // TODO: Update placeholder value.

            // How values should be represented in the output.
            // The default render option is ValueRenderOption.FORMATTED_VALUE.
            SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum valueRenderOption = (SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum)0;  // TODO: Update placeholder value.

            // How dates, times, and durations should be represented in the output.
            // This is ignored if value_render_option is
            // FORMATTED_VALUE.
            // The default dateTime render option is [DateTimeRenderOption.SERIAL_NUMBER].
            SpreadsheetsResource.ValuesResource.BatchGetRequest.DateTimeRenderOptionEnum dateTimeRenderOption = (SpreadsheetsResource.ValuesResource.BatchGetRequest.DateTimeRenderOptionEnum)0;  // TODO: Update placeholder value.

            SpreadsheetsResource.ValuesResource.BatchGetRequest request = GetSheetsService().Spreadsheets.Values.BatchGet(spreadsheetId);
            request.Ranges = range;
            request.ValueRenderOption = valueRenderOption;
            request.DateTimeRenderOption = dateTimeRenderOption;

            // To execute asynchronously in an async method, replace `request.Execute()` as shown:
            Data.BatchGetValuesResponse response = request.Execute();
            // Data.BatchGetValuesResponse response = await request.ExecuteAsync();

            // TODO: Change code below to process the `response` object:
            var data = response.ValueRanges[0].Values;
            foreach (var s in data)
            {
                string datasss = "";
                for (int i = 0; i < s.Count; i++)
                {
                    string dataGG = s[i].ToString();
                    if (i == 0)
                    {
                        datasss = dataGG;
                    }
                    else
                    {
                        datasss = datasss + "|" + dataGG;
                    }
                }
                list.Add(datasss);
            }

        }
        public void ClearCache(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        public void getLastLineGoogleSheet(string spreadsheetId, string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum valueRenderOption = (SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum)0;
            SpreadsheetsResource.ValuesResource.GetRequest.DateTimeRenderOptionEnum dateTimeRenderOption = (SpreadsheetsResource.ValuesResource.GetRequest.DateTimeRenderOptionEnum)0;
            SpreadsheetsResource.ValuesResource.GetRequest request = GetSheetsService().Spreadsheets.Values.Get(spreadsheetId, range);

            request.ValueRenderOption = valueRenderOption;
            request.DateTimeRenderOption = dateTimeRenderOption;
            try
            {
                // To execute asynchronously in an async method, replace `request.Execute()` as shown:
                Data.ValueRange response = request.Execute();
                // Data.ValueRange response = await request.ExecuteAsync();

                // TODO: Change code below to process the `response` object:
                //Console.WriteLine(JsonConvert.SerializeObject(response));

                IList<IList<Object>> values = response.Values;
                if (values != null)
                {
                    number = values.Count;
                }
                else
                {
                    number = 1;
                }
                //res(true, values.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:{ex.Message}");
            }
        }
    }
}
