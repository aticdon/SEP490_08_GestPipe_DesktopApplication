using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GestPipePowerPonit.Models;
using System;

namespace GestPipePowerPonit.Services
{
    public class TopicService
    {
        private readonly HttpClient client;

        public TopicService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
        }

        public async Task<List<Topic>> GetTopicsAsync()
        {
            var response = await client.GetAsync("api/Topic");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Topic>>(json);
        }
    }
}