using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class UserGestureConfigService
    {
        private readonly HttpClient _httpClient;

        public UserGestureConfigService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7219/");
        }
        public async Task<List<UserGestureConfigDto>> GetUserGesturesAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<List<UserGestureConfigDto>>($"/api/usergestureconfig/user/{userId}");
        }
        public async Task<UserGestureConfig> GetUserGestureByid(string Id)
        {
            return await _httpClient.GetFromJsonAsync<UserGestureConfig>($"/api/usergestureconfig/{Id}");
        }

        public async Task<GestureDetailsDto> GetGestureDetailAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<GestureDetailsDto>($"/api/usergestureconfig/{id}/detail");
        }

        public string GetGestureDescription(GestureDetailsDto gesture)
        {
            string leftHand = GetHandState(
                gesture.VectorData.Fingers,
                0,
                I18nHelper.GetString("Left hand", "Tay trái")
            );
            string rightHand = GetHandState(
                gesture.VectorData.Fingers,
                5,
                I18nHelper.GetString("Right hand", "Tay phải")
            );

            string typeName = I18nHelper.GetLocalized(gesture.Type);
            string direction = "";
            if (typeName == I18nHelper.GetString("Static", "Tĩnh"))
            {
                direction = I18nHelper.GetString(
                    "Keep right hand still for 1 second.",
                    "Giữ tay phải cố định 1 giây."
                );
            }
            else
            {
                if (gesture.VectorData.MainAxisX == 1)
                    direction = gesture.VectorData.DeltaX > 0
                        ? I18nHelper.GetString("Then wave hand from left to right.", "Vẫy tay từ trái sang phải.")
                        : I18nHelper.GetString("Then wave hand from right to left.", "Vẫy tay từ phải sang trái.");
                else if (gesture.VectorData.MainAxisY == 1)
                    direction = gesture.VectorData.DeltaY > 0
                        ? I18nHelper.GetString("Then wave hand from top to bottom.", "Vẫy tay từ trên xuống dưới.")
                        : I18nHelper.GetString("Then wave hand from bottom to top.", "Vẫy tay từ dưới lên trên.");
            }
            string finish = I18nHelper.GetString(
                "Left hand straighten fingers to finish the movement.",
                "Duỗi các ngón tay trái để hoàn thành động tác."
            );

            return $"{leftHand}{Environment.NewLine}" +
                   $"{rightHand}{Environment.NewLine}" +
                   $"{direction}{Environment.NewLine}" +
                   $"{finish}";
        }

        private string GetHandState(int[] fingers, int start, string handLabel)
        {
            string[] fingerNamesEn = { "thumb", "index", "middle", "ring", "pinky" };
            string[] fingerNamesVi = { "ngón cái", "ngón trỏ", "ngón giữa", "ngón áp út", "ngón út" };
            string[] fingerNames = I18nHelper.GetLang() == "vi" ? fingerNamesVi : fingerNamesEn;

            List<string> open = new List<string>();
            List<string> closed = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                if (fingers[start + i] == 1)
                    open.Add(fingerNames[i]);
                else
                    closed.Add(fingerNames[i]);
            }
            if (open.Count == 0)
                return $"{handLabel} {I18nHelper.GetString("clenched.", "nắm lại.")}";
            if (closed.Count == 0)
                return $"{handLabel} {I18nHelper.GetString("all fingers open.", "các ngón đều duỗi.")}";
            return $"{handLabel} {I18nHelper.GetString("open", "duỗi")} {string.Join(" + ", open)}.";
        }

        // Hàm instruction dạng bảng
        public string GetInstructionTable(GestureDetailsDto gesture)
        {
            string[] fingerNamesEn = { "Thumb", "Index Finger", "Middle Finger", "Ring Finger", "Little Finger" };
            string[] fingerNamesVi = { "Ngón cái", "Ngón trỏ", "Ngón giữa", "Ngón áp út", "Ngón út" };
            string[] fingerNames = I18nHelper.GetLang() == "vi" ? fingerNamesVi : fingerNamesEn;

            string[] leftHandState = new string[5];
            string[] rightHandState = new string[5];
            for (int i = 0; i < 5; i++)
            {
                leftHandState[i] = gesture.VectorData.Fingers[i] == 1 ? I18nHelper.GetString("Open", "Duỗi") : I18nHelper.GetString("Close", "Nắm");
                rightHandState[i] = gesture.VectorData.Fingers[i + 5] == 1 ? I18nHelper.GetString("Open", "Duỗi") : I18nHelper.GetString("Close", "Nắm");
            }

            string typeName = I18nHelper.GetLocalized(gesture.Type);
            string direction = "";
            if (typeName == I18nHelper.GetString("Static", "Tĩnh"))
            {
                direction = I18nHelper.GetString("Stand Still", "Đứng yên");
            }
            else
            {
                if (gesture.VectorData.MainAxisX == 1)
                    direction = gesture.VectorData.DeltaX > 0 ? I18nHelper.GetString("Left to Right", "Trái sang phải") : I18nHelper.GetString("Right to Left", "Phải sang trái");
                else if (gesture.VectorData.MainAxisY == 1)
                    direction = gesture.VectorData.DeltaY > 0 ? I18nHelper.GetString("Top to Bottom", "Trên xuống dưới") : I18nHelper.GetString("Bottom to Top", "Dưới lên trên");
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{"",-16}{I18nHelper.GetString("Left Hands", "Tay trái"),-12}{I18nHelper.GetString("Right Hands", "Tay phải"),-12}");
            for (int i = 0; i < 5; i++)
                sb.AppendLine($"{fingerNames[i],-16}{leftHandState[i],-12}{rightHandState[i],-12}");
            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"{I18nHelper.GetString("Direction:", "Hướng di chuyển:"),-16}{direction}");
            return sb.ToString();
        }
        //public async Task<int> ImportFromCsvAsync(string userId, string csvPath)
        //{
        //    if (!File.Exists(csvPath))
        //        throw new FileNotFoundException("CSV file not found", csvPath);

        //    // Đọc nội dung file CSV do Python đã sinh ra
        //    string csvContent = File.ReadAllText(csvPath, Encoding.UTF8);

        //    var request = new ImportGestureFromCsvRequest
        //    {
        //        UserId = userId,
        //        CsvContent = csvContent
        //    };

        //    // Gọi API backend: POST /api/usergestureconfig/import-from-csv
        //    var response = await _httpClient.PostAsJsonAsync(
        //        "/api/usergestureconfig/import-from-csv",
        //        request
        //    );

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"ImportFromCsv failed: {(int)response.StatusCode} - {error}");
        //    }

        //    // Backend trả { inserted: n }
        //    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        //    return result != null && result.TryGetValue("inserted", out var count)
        //        ? count
        //        : 0;
        //}
        public async Task<int> ImportFromCsvAsync(string userId, string csvContent)
        {
            var payload = new ImportGestureFromCsvRequest
            {
                UserId = userId,
                CsvContent = csvContent
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/api/usergestureconfig/import-from-csv",
                payload
            );

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"ImportFromCsv failed: {response.StatusCode} - {body}");
            }

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            // backend trả: { inserted = count }
            return (int)result.inserted;
        }

    }
}