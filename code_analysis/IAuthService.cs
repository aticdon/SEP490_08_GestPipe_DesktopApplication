using GestPipe.Backend.Models;
using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Models.DTOs.Auth;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string ipAddress, string userAgent);
        Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<AuthResponseDto> GoogleLoginAsync(string idToken, string ipAddress, string userAgent);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(string userId);
        Task<AuthResponseDto> ForgotPasswordAsync(string email);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetDto);
        Task<AuthResponseDto> LogoutAsync(string userId);
    }
}   