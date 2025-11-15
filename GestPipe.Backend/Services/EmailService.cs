using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendVerificationEmailAsync(string email, string otp)
        {
            var subject = "Xác thực tài khoản GestPipe";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                        <h2 style='color: #333; text-align: center;'>Xác thực địa chỉ email</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản GestPipe. Vui lòng sử dụng mã xác thực sau để hoàn tất quá trình đăng ký:</p>
                        <div style='margin: 25px 0; background-color: #f5f5f5; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; border-radius: 4px;'>
                            {otp}
                        </div>
                        <p>Mã xác thực này sẽ hết hạn sau 5 phút.</p>
                        <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.</p>
                        <p>Trân trọng,<br>Đội ngũ GestPipe</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string? name)
        {
            var displayName = !string.IsNullOrEmpty(name) ? name : "bạn";

            var subject = "Chào mừng đến với GestPipe!";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                        <h2 style='color: #333; text-align: center;'>Chào mừng đến với GestPipe!</h2>
                        <p>Xin chào {displayName},</p>
                        <p>Cảm ơn bạn đã hoàn tất đăng ký tài khoản GestPipe. Tài khoản của bạn đã được xác thực thành công và đã sẵn sàng sử dụng.</p>
                        <p>Bạn có thể đăng nhập ngay bây giờ để bắt đầu sử dụng các dịch vụ của chúng tôi.</p>
                        <p>Nếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>
                        <p>Trân trọng,<br>Đội ngũ GestPipe</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string otp)
        {
            var subject = "Yêu cầu đặt lại mật khẩu GestPipe";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                        <h2 style='color: #333; text-align: center;'>Đặt lại mật khẩu</h2>
                        <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Vui lòng sử dụng mã xác thực sau để đặt lại mật khẩu:</p>
                        <div style='margin: 25px 0; background-color: #f5f5f5; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; border-radius: 4px;'>
                            {otp}
                        </div>
                        <p>Mã xác thực này sẽ hết hạn sau 5 phút.</p>
                        <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với bộ phận hỗ trợ nếu bạn có bất kỳ thắc mắc nào.</p>
                        <p>Trân trọng,<br>Đội ngũ GestPipe</p>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.From),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; // Re-throw to be handled by the calling method
            }
        }
        public async Task SendPasswordResetConfirmationEmailAsync(string email)
        {
            var subject = "Mật khẩu GestPipe đã được thay đổi";
            var body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                <h2 style='color: #333; text-align: center;'>Xác nhận thay đổi mật khẩu</h2>
                <p>Mật khẩu cho tài khoản của bạn vừa được thay đổi thành công. Nếu bạn không thực hiện thay đổi này, vui lòng liên hệ ngay với đội ngũ hỗ trợ của chúng tôi.</p>
                <p>Trân trọng,<br/>Đội ngũ GestPipe</p>
            </div>
        </body>
        </html>";

            await SendEmailAsync(email, subject, body);
        }
    }
}