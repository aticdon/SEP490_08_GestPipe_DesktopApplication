using GestPipe.Backend.Models.DTOs.Auth;
using GestPipe.Backend.Models.DTOs.Profile;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Interfaces
{
    public interface IProfileService
    {
        Task<AuthResponseDto> GetUserProfileAsync(string userId);
        Task<AuthResponseDto> UpdateProfileAsync(string userId, UpdateProfileDto updateDto);
        Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
    }
}