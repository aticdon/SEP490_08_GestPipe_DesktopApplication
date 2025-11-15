using System;
using System.Threading.Tasks;
using GestPipePowerPonit.Models.DTOs;

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
    }
}