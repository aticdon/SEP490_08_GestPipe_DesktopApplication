using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GestPipePowerPonit.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7000/api/";

        public ApiService()
        {
            // 👇 THÊM IGNORE SSL - Dùng cho localhost (xóa khi production)
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30) // 👈 SET TIMEOUT
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Nếu có token, tự động thêm vào header
            var token = Properties.Settings.Default.AuthToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data) where T : class
        {
            try
            {
                // 👇 DEBUG INFO
                Console.WriteLine($"[ApiService] POST {BaseUrl}{endpoint}");

                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                Console.WriteLine($"[ApiService] Request Body: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // 👇 GỬI REQUEST
                Console.WriteLine($"[ApiService] Sending request...");
                var response = await _httpClient.PostAsync(endpoint, content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ApiService] Response Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                Console.WriteLine($"[ApiService] Response Body: {responseContent}");

                // 👇 KIỂM TRA RESPONSE
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ApiService] Success! Parsing response...");
                    try
                    {
                        var result = JsonConvert.DeserializeObject<T>(responseContent);
                        Console.WriteLine($"[ApiService] Parsed successfully");
                        return result;
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"[ApiService] JSON Parse Error: {jsonEx.Message}");
                        throw new Exception($"Failed to parse response: {jsonEx.Message}");
                    }
                }
                else
                {
                    // 👇 ERROR RESPONSE
                    Console.WriteLine($"[ApiService] Error response received");
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<T>(responseContent);
                        return errorResponse;
                    }
                    catch
                    {
                        throw new Exception($"API Error: {(int)response.StatusCode} {response.ReasonPhrase}\n{responseContent}");
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[ApiService] HttpRequestException: {httpEx.Message}");
                throw new Exception($"Network error: {httpEx.Message}", httpEx);
            }
            catch (TaskCanceledException timeoutEx)
            {
                Console.WriteLine($"[ApiService] TaskCanceledException (Timeout): {timeoutEx.Message}");
                throw new Exception($"Request timeout: {timeoutEx.Message}", timeoutEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApiService] General Exception: {ex.GetType().Name}");
                Console.WriteLine($"[ApiService] Message: {ex.Message}");
                Console.WriteLine($"[ApiService] StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public void SetAuthToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"[ApiService] Clearing auth token");
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                Console.WriteLine($"[ApiService] Setting auth token");
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public void ClearAuthToken()
        {
            Console.WriteLine($"[ApiService] ClearAuthToken called");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}