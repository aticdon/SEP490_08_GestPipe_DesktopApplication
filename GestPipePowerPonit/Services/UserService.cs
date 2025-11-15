using GestPipePowerPonit.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class UserService
    {
        private readonly HttpClient client;
        public UserService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
        }

        // Kiểm tra trạng thái request của user
        //public async Task<bool> CheckCanRequestAsync(string userId)
        //{
        //    try
        //    {
        //        var response = await client.GetAsync($"/api/User/{userId}");
        //        if (!response.IsSuccessStatusCode)
        //            return false;

        //        var userJson = await response.Content.ReadAsStringAsync();
        //        var userDto = JsonConvert.DeserializeObject<UserRequestDto>(userJson);
        //        return userDto != null &&
        //            string.Equals(userDto.GestureRequestStatus, "enabled", StringComparison.OrdinalIgnoreCase);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public async Task<bool> CheckCanRequestAsync(string userId)
        {
            try
            {
                var response = await client.GetAsync($"/api/User/{userId}");
                if (!response.IsSuccessStatusCode)
                    return false;

                var userJson = await response.Content.ReadAsStringAsync();
                var userDto = JsonConvert.DeserializeObject<UserRequestDto>(userJson);

                // Điều kiện: Nếu bị disable hoặc đã request đủ 3 lần thì trả về false
                if (userDto == null)
                    return false;

                bool isDisabled = string.Equals(userDto.GestureRequestStatus, "disable", StringComparison.OrdinalIgnoreCase);
                bool isExceededCount = userDto.RequestCountToday >= 3;

                return !isDisabled && !isExceededCount;
            }
            catch
            {
                return false;
            }
        }

        // Tăng request_count_today
        public async Task<bool> IncrementRequestCountAsync(string userId)
        {
            try
            {
                var response = await client.PostAsync($"/api/User/{userId}/increment-request-count", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // Đổi gesture_request_status
        public async Task<bool> UpdateGestureRequestStatusAsync(string userId, string status)
        {
            try
            {
                var json = JsonConvert.SerializeObject(status); // status là "disable" hoặc "enabled"
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"/api/User/{userId}/update-request-status", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}