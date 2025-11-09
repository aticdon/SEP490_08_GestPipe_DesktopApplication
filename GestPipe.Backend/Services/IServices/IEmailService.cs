using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string otp);
        Task SendWelcomeEmailAsync(string email, string? name);
        Task SendPasswordResetEmailAsync(string email, string otp);
        Task SendPasswordResetConfirmationEmailAsync(string toEmail);
    }
}