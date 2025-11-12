using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GestPipePowerPonit.Models;
using System;

namespace GestPipePowerPonit.Services
{
    public class CategoryService
    {
        private readonly HttpClient client;

        public CategoryService()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7219/");
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await client.GetAsync("api/Category");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Category>>(json);
        }
    }
}