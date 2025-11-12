using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GestPipePowerPonit.Models;

namespace GestPipePowerPonit.Services
{
    public class SessionService
    {
        private readonly HttpClient client;

        public SessionService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
        }

        public async Task<bool> SaveSessionAsync(Session session)
        {
            var json = JsonConvert.SerializeObject(session);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Session", content);
            return response.IsSuccessStatusCode;
        }
    }
}