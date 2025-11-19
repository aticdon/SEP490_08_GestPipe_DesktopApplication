using GestPipePowerPonit.Models.DTOs;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class ProfileService
    {
        private readonly ApiService _apiService;

        public ProfileService()
        {
            _apiService = new ApiService();
        }

        public async Task<ProfileResponseDto> GetProfileAsync(string userId)
        {
            try
            {
                Console.WriteLine($"[ProfileService] Getting profile for user: {userId}");
                return await _apiService.GetAsync<ProfileResponseDto>("profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProfileService] GetProfile Error: {ex.Message}");
                return new ProfileResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ProfileResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateDto)
        {
            try
            {
                Console.WriteLine($"[ProfileService] Updating profile for user: {userId}");
                return await _apiService.PutAsync<ProfileResponseDto>("profile", updateDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProfileService] UpdateProfile Error: {ex.Message}");
                return new ProfileResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                Console.WriteLine($"[ProfileService] Changing password for user: {userId}");
                return await _apiService.PostAsync<AuthResponseDto>("profile/change-password", changePasswordDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProfileService] ChangePassword Error: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<string> UploadAvatarAsync(string filePath)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslErrors) => true
                };

                using (var client = new HttpClient(handler))
                using (var form = new MultipartFormDataContent())
                {
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);
                    var fileContent = new ByteArrayContent(fileBytes);

                    string contentType = "image/jpeg";
                    string ext = System.IO.Path.GetExtension(filePath).ToLower();
                    if (ext == ".png") contentType = "image/png";
                    else if (ext == ".jpg" || ext == ".jpeg") contentType = "image/jpeg";

                    fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);

                    // ✅ THỬ CẢ 2: "file" (thường) hoặc "File" (hoa)
                    form.Add(fileContent, "File", System.IO.Path.GetFileName(filePath));

                    Console.WriteLine($"[ProfileService] Uploading: {filePath} ({fileBytes.Length} bytes)");

                    var response = await client.PostAsync("https://localhost:7219/api/upload/avatar", form);

                    Console.WriteLine($"[ProfileService] Status: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[ProfileService] Response: {json}");

                        using (var doc = System.Text.Json.JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;
                            if (root.TryGetProperty("url", out var urlElement))
                                return urlElement.GetString();
                        }
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[ProfileService] Error: {error}");
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileService] Exception: {ex.Message}");
                return null;
            }
        }
        public async Task<ProfileResponseDto> UpdateAvatarOnlyAsync(string userId, string avatarUrl)
        {
            try
            {
                Console.WriteLine($"[ProfileService] Updating avatar only for user: {userId}");

                var payload = new { AvatarUrl = avatarUrl };
                var response = await _apiService.PatchAsync<ProfileResponseDto>("profile/avatar", payload);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ProfileService] UpdateAvatarOnly Error: {ex.Message}");
                return new ProfileResponseDto { Success = false, Message = ex.Message };
            }
        }
    }
}