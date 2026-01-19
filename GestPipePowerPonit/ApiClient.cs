using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GestPipePowerPonit.Models;

namespace GestPipePowerPonit
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _http;

        public ApiClient(string baseAddress, HttpMessageHandler handler = null)
        {
            _http = handler == null ? new HttpClient() : new HttpClient(handler);
            _http.BaseAddress = new Uri(baseAddress.TrimEnd('/'));
        }

        public async Task<UserDto> GetUserAsync(string id)
        {
            var resp = await _http.GetAsync($"/api/user/{id}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<UserDto>();
        }

        public async Task SetUserLanguageAsync(string id, string language)
        {
            var resp = await _http.PostAsJsonAsync($"/api/user/{id}/language", language);
            resp.EnsureSuccessStatusCode();
        }

        public void Dispose() => _http?.Dispose();
    }
}