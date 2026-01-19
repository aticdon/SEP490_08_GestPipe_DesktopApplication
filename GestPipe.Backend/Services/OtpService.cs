using GestPipe.Backend.Models;
using GestPipe.Backend.Services.IServices;
using MongoDB.Driver;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace GestPipe.Backend.Services.Implementation
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<Otp> _otpCollection;
        private readonly int _otpExpiryMinutes = 5;


        public OtpService(IMongoDatabase database)
        {
            _otpCollection = database.GetCollection<Otp>("Otp");


            // TTL index để auto xóa OTP hết hạn
            var indexKeysDefinition = Builders<Otp>.IndexKeys.Ascending(x => x.ExpiresAt);
            var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            var indexModel = new CreateIndexModel<Otp>(indexKeysDefinition, indexOptions);
            _ = _otpCollection.Indexes.CreateOneAsync(indexModel); // fire-and-forget
        }


        private static string NormalizeEmail(string email)
            => email?.Trim().ToLowerInvariant();


        public async Task<string> GenerateOtpAsync(string userId, string email, string purpose)
        {
            var normalizedEmail = NormalizeEmail(email);


            // Chống spam gửi OTP
            if (await IsOtpLimitExceededAsync(normalizedEmail))
                throw new Exception("OTP request limit exceeded. Please try again later.");


            var otpCode = GenerateRandomOtp();


            var newOtp = new Otp
            {
                Email = normalizedEmail,
                OtpCode = otpCode,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes),
                AttemptsCount = 0
            };


            // Replace OTP cũ nếu tồn tại, tránh nhiều OTP cho cùng email
            await _otpCollection.ReplaceOneAsync(
                x => x.Email == normalizedEmail,
                newOtp,
                new ReplaceOptions { IsUpsert = true });


            return otpCode;
        }


        public async Task<bool> ValidateOtpAsync(string email, string otpCode, string purpose)
        {
            var normalizedEmail = NormalizeEmail(email);


            var otp = await _otpCollection
                .Find(x => x.Email == normalizedEmail)
                .FirstOrDefaultAsync();


            if (otp == null ||
                otp.OtpCode != otpCode ||
                otp.Purpose != purpose ||
                otp.IsExpired())
            {
                return false;
            }


            // Tăng số lần thử (để sau này có thể giới hạn theo AttemptsCount nếu muốn)
            var update = Builders<Otp>.Update.Inc(x => x.AttemptsCount, 1);
            await _otpCollection.UpdateOneAsync(x => x.Email == normalizedEmail, update);


            return true;
        }


        public async Task<bool> IsOtpLimitExceededAsync(string email)
        {
            var normalizedEmail = NormalizeEmail(email);


            var otp = await _otpCollection
                .Find(x => x.Email == normalizedEmail)
                .FirstOrDefaultAsync();


            if (otp == null)
                return false;


            // Nếu OTP hiện tại được tạo trong vòng 60 giây gần đây => coi như spam
            var timeSinceCreation = DateTime.UtcNow - otp.CreatedAt;
            return timeSinceCreation.TotalSeconds < 60;
        }


        public async Task<bool> DeleteOtpAsync(string email)
        {
            var normalizedEmail = NormalizeEmail(email);
            var result = await _otpCollection.DeleteOneAsync(x => x.Email == normalizedEmail);
            return result.DeletedCount > 0;
        }


        private string GenerateRandomOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            uint randomNumber = BitConverter.ToUInt32(bytes, 0);
            int otpNumber = (int)(randomNumber % 1_000_000);
            return otpNumber.ToString("D6");
        }
    }
}