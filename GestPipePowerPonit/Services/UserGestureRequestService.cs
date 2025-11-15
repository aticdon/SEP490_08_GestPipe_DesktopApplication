//using GestPipePowerPonit.Models.DTOs;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Text;
//using System.Threading.Tasks;

//namespace GestPipePowerPonit.Services
//{
//    public class UserGestureRequestService
//    {
//        private readonly HttpClient client;
//        public UserGestureRequestService()
//        {
//            client = new HttpClient();
//            client.BaseAddress = new Uri("https://localhost:7219/");
//        }
//        public async Task<bool> CreateRequestAsync(UserGestureRequestDto dto)
//        {
//            try
//            {
//                var json = JsonConvert.SerializeObject(dto);
//                var content = new StringContent(json, Encoding.UTF8, "application/json");
//                var response = await client.PostAsync("/api/UserGestureRequest", content);
//                return response.IsSuccessStatusCode;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<UserGestureRequestDto> GetLatestRequestByConfigAsync(string configId)
//        {
//            try
//            {
//                // Gọi API lấy request mới nhất cho UserGestureConfigId
//                var response = await client.GetAsync($"/api/UserGestureRequest/config/{configId}");
//                if (response.IsSuccessStatusCode)
//                {
//                    // Nếu có request (status 200)
//                    return await response.Content.ReadFromJsonAsync<UserGestureRequestDto>();
//                }
//                else
//                {
//                    // Nếu không có (404 hoặc lỗi)
//                    return null;
//                }
//            }
//            catch
//            {
//                return null;
//            }
//        }
//    }
//}

using GestPipePowerPonit.Models.DTOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class UserGestureRequestService
    {
        private readonly HttpClient client;
        public UserGestureRequestService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
        }
        public async Task<bool> CreateRequestAsync(UserGestureRequestDto dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/UserGestureRequest", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserGestureRequestDto> GetLatestRequestByConfigAsync(string configId)
        {
            try
            {
                // Gọi API lấy request mới nhất cho UserGestureConfigId
                var response = await client.GetAsync($"/api/UserGestureRequest/config/{configId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserGestureRequestDto>();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        // NEW: Đổi trạng thái request sang Training
        public async Task<bool> SetRequestStatusToTrainingAsync(string requestId)
        {
            try
            {
                var response = await client.PostAsync($"/api/UserGestureRequest/{requestId}/start-training", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // NEW: Đổi trạng thái request sang Successful
        public async Task<bool> SetTrainingToSuccessfulAsync(string requestId)
        {
            try
            {
                var response = await client.PostAsync($"/api/UserGestureRequest/{requestId}/complete", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}