using GestPipe.Backend.Models;
using GestPipe.Backend.Services.Interfaces;
using MongoDB.Driver;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Implementation
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<Otp> _otpCollection;
        private readonly int _otpLength = 6;
        private readonly int _otpExpiryMinutes = 5;

        public OtpService(IMongoDatabase database)
        {
            _otpCollection = database.GetCollection<Otp>("Otp");

            // Tạo TTL Index để tự động xóa OTP hết hạn
            var indexKeysDefinition = Builders<Otp>.IndexKeys.Ascending(x => x.ExpiresAt);
            var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            var indexModel = new CreateIndexModel<Otp>(indexKeysDefinition, indexOptions);
            _otpCollection.Indexes.CreateOneAsync(indexModel);
        }

        public async Task<string> GenerateOtpAsync(string userId, string email, string purpose)
        {
            // Kiểm tra giới hạn gửi OTP
            if (await IsOtpLimitExceededAsync(email))
                throw new Exception("OTP request limit exceeded. Please try again later.");

            // Tạo mã OTP mới
            var otpCode = GenerateRandomOtp();

            // Tìm OTP cũ theo email
            var existingOtp = await _otpCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

            var newOtp = new Otp
            {
                Email = email,
                OtpCode = otpCode,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes),
                AttemptsCount = 0
            };

            if (existingOtp != null)
            {
                // Cập nhật OTP cũ (Replace toàn bộ document)
                await _otpCollection.ReplaceOneAsync(x => x.Email == email, newOtp);
            }
            else
            {
                // Insert OTP mới
                await _otpCollection.InsertOneAsync(newOtp);
            }

            return otpCode;
        }

        public async Task<bool> ValidateOtpAsync(string email, string otpCode, string purpose)
        {
            var otp = await _otpCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

            if (otp == null ||
                otp.OtpCode != otpCode ||
                otp.Purpose != purpose ||
                otp.IsExpired())
            {
                return false;
            }

            // Tăng số lần thử
            var update = Builders<Otp>.Update.Inc(x => x.AttemptsCount, 1);
            await _otpCollection.UpdateOneAsync(x => x.Email == email, update);

            return true;
        }

        public async Task<bool> IsOtpLimitExceededAsync(string email)
        {
            var otp = await _otpCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

            if (otp == null)
                return false;

            // Nếu OTP hiện tại được tạo trong vòng 1 phút gần đây => có thể đang spam
            var timeSinceCreation = DateTime.UtcNow - otp.CreatedAt;
            if (timeSinceCreation.TotalSeconds < 60)
                return true;

            return false;
        }

        public async Task<bool> MarkOtpAsUsedAsync(string email, string otpCode)
        {
            var otp = await _otpCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

            if (otp == null || otp.OtpCode != otpCode)
                return false;

            // Xóa OTP sau khi sử dụng
            var result = await _otpCollection.DeleteOneAsync(x => x.Email == email);
            return result.DeletedCount > 0;
        }

        public async Task<bool> MarkOtpAsVerifiedAsync(string email, string otpCode)
        {
            var otp = await _otpCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

            if (otp == null || otp.OtpCode != otpCode)
                return false;

            // Tăng attempt count
            var update = Builders<Otp>.Update.Inc(x => x.AttemptsCount, 1);
            var result = await _otpCollection.UpdateOneAsync(x => x.Email == email, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteOtpAsync(string email)
        {
            var result = await _otpCollection.DeleteOneAsync(x => x.Email == email);
            return result.DeletedCount > 0;
        }

        private string GenerateRandomOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            uint randomNumber = BitConverter.ToUInt32(bytes, 0);
            int otpNumber = (int)(randomNumber % 1000000);
            return otpNumber.ToString("D6");
        }
    }
}