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
        //public async Task<bool> CreateRequestAsync(UserGestureRequestDto dto)
        //{
        //    try
        //    {
        //        var json = JsonConvert.SerializeObject(dto);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync("/api/UserGestureRequest", content);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        // File: GestPipePowerPonit/Services/UserGestureRequestService. cs
        public async Task<bool> CreateRequestAsync(UserGestureRequestDto dto)
        {
            try
            {
                // ✅ LOG request data
                Console.WriteLine($"[CreateRequest] Request data:");
                Console.WriteLine($"  - UserId: {dto.UserId}");
                Console.WriteLine($"  - GestureTypeId: {dto.GestureTypeId}");
                Console.WriteLine($"  - UserGestureConfigId: {dto.UserGestureConfigId}");
                Console.WriteLine($"  - PoseLabel: {dto.PoseLabel}");
                Console.WriteLine($"  - Status: {JsonConvert.SerializeObject(dto.Status)}");

                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/UserGestureRequest", content);

                // ✅ LOG response
                Console.WriteLine($"[CreateRequest] Response: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[CreateRequest] Error body: {errorBody}");

                    // ✅ Show error to user
                    throw new Exception($"Server error ({response.StatusCode}): {errorBody}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateRequest] Exception: {ex.Message}");
                Console.WriteLine($"[CreateRequest] StackTrace: {ex.StackTrace}");
                throw; // ✅ Propagate exception
            }
        }

        public async Task<UserGestureRequestDto> GetLatestRequestByConfigAsync(string configId, string userId)
        {
            try
            {
                // Gọi API lấy request mới nhất cho UserGestureConfigId
                var response = await client.GetAsync($"/api/UserGestureRequest/user/{userId}/config/{configId}");
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
        // ✅ THAY ĐỔI: Thêm parameter userId
        public async Task<bool> SetRequestStatusToTrainingAsync(string configId, string userId)
        {
            try
            {
                Console.WriteLine($"[Frontend] SetRequestStatusToTraining - configId: {configId}, userId: {userId}");

                // ✅ URL mới với userId
                var url = $"/api/UserGestureRequest/user/{userId}/config/{configId}/start-training";

                var response = await client.PostAsync(url, null);

                Console.WriteLine($"[Frontend] Response: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Frontend] Error: {errorBody}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Frontend] Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SetTrainingToSuccessfulAsync(string configId, string userId)
        {
            try
            {
                Console.WriteLine($"[Frontend] SetTrainingToSuccessful - configId: {configId}, userId: {userId}");

                var url = $"/api/UserGestureRequest/user/{userId}/config/{configId}/complete";

                var response = await client.PostAsync(url, null);

                Console.WriteLine($"[Frontend] Response: {response.StatusCode}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Frontend] Exception: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> SetTrainingToCanceledAsync(string configId, string userId)
        {
            try
            {
                Console.WriteLine($"[Frontend] SetTrainingToCanceled - configId: {configId}, userId: {userId}");

                var url = $"/api/UserGestureRequest/user/{userId}/config/{configId}/cancel";

                var response = await client.PostAsync(url, null);

                Console.WriteLine($"[Frontend] Response: {response.StatusCode}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Frontend] Exception: {ex.Message}");
                return false;
            }
        }
        public async Task<List<UserGestureRequestDto>> GetLatestRequestsBatchAsync(string userId, List<string> gestureConfigIds)
        {
            try
            {
                var data = new
                {
                    UserId = userId,
                    GestureConfigIds = gestureConfigIds
                };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/UserGestureRequest/batch/latest-requests", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<List<UserGestureRequestDto>>();
                    return result;
                }
                return new List<UserGestureRequestDto>();
            }
            catch
            {
                return new List<UserGestureRequestDto>();
            }
        }
    }
}