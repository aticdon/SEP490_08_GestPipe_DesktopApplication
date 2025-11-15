using GestPipe.Backend.Models.DTOs.Auth;
using GestPipe.Backend.Models.DTOs.ProfileUser;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.IServices
{
    public interface IProfileService
    {
        // ✅ THAY ĐỔI: AuthResponseDto → ProfileResponseDto
        Task<ProfileResponseDto> GetUserProfileAsync(string userId);
        Task<ProfileResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateDto);

        // ✅ GIỮ NGUYÊN: ChangePassword vẫn dùng AuthResponseDto
        Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    }
}