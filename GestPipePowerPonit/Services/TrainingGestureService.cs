using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GestPipePowerPonit.Models;

namespace GestPipePowerPonit.Services
{
    public class TrainingGestureService
    {
        private readonly HttpClient _httpClient;
        public TrainingGestureService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7219/");
        }
        public async Task<bool> SaveTrainingGestureAsync(TrainingGesture result)
        {
            var json = JsonConvert.SerializeObject(result);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/TrainingGesture", content);

            return response.IsSuccessStatusCode;
        }
    }
}
