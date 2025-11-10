using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string userId, string email, string purpose);
        Task<bool> ValidateOtpAsync(string email, string otpCode, string purpose);
        Task<bool> IsOtpLimitExceededAsync(string email);
        Task<bool> MarkOtpAsUsedAsync(string email, string otpCode);
        Task<bool> MarkOtpAsVerifiedAsync(string email, string otpCode);
    }
}
