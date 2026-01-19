using System.Threading.Tasks;


namespace GestPipe.Backend.Services.IServices
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string userId, string email, string purpose);
        Task<bool> ValidateOtpAsync(string email, string otpCode, string purpose);
        Task<bool> IsOtpLimitExceededAsync(string email);
        Task<bool> DeleteOtpAsync(string email);
    }
}





