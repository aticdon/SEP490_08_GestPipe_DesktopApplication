using GestPipe.Backend.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GestPipe.Backend.Services.Implementation
{
    public class OtpService : IOtpService
    {
        private class OtpEntry
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string OtpCode { get; set; }
            public string Purpose { get; set; }
            public DateTime ExpiresAt { get; set; }
            public bool IsUsed { get; set; } = false;
            public int AttemptsCount { get; set; } = 0;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? VerifiedAt { get; set; } = null;
        }

        // Bộ nhớ tạm lưu OTP trong RAM
        private static readonly ConcurrentDictionary<string, OtpEntry> _otpStorage = new();

        private readonly int _otpLength = 6;
        private readonly int _otpExpiryMinutes = 5;
        private readonly int _maxOtpPerHour = 3;

        public async Task<string> GenerateOtpAsync(string userId, string email, string purpose)
        {
            // Kiểm tra giới hạn gửi OTP
            if (await IsOtpLimitExceededAsync(email))
                throw new Exception("OTP request limit exceeded. Please try again later.");

            // Xóa OTP cũ cho cùng email/purpose
            var existing = _otpStorage.Values
                .Where(o => o.Email == email && o.Purpose == purpose && !o.IsUsed)
                .ToList();
            foreach (var item in existing)
            {
                _otpStorage.TryRemove(GetKey(item.Email, item.OtpCode), out _);
            }

            // Tạo mã OTP mới
            var otpCode = GenerateRandomOtp();

            var otp = new OtpEntry
            {
                UserId = userId,
                Email = email,
                OtpCode = otpCode,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes)
            };

            _otpStorage[GetKey(email, otpCode)] = otp;

            return await Task.FromResult(otpCode);
        }

        public async Task<bool> ValidateOtpAsync(string email, string otpCode, string purpose)
        {
            _otpStorage.TryGetValue(GetKey(email, otpCode), out var otp);

            if (otp == null || otp.IsUsed || otp.Purpose != purpose || otp.ExpiresAt < DateTime.UtcNow)
                return await Task.FromResult(false);

            otp.AttemptsCount++;
            return await Task.FromResult(true);
        }

        public async Task<bool> IsOtpLimitExceededAsync(string email)
        {
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            var count = _otpStorage.Values.Count(o =>
                o.Email == email &&
                o.CreatedAt > oneHourAgo);

            return await Task.FromResult(count >= _maxOtpPerHour);
        }

        public async Task<bool> MarkOtpAsUsedAsync(string email, string otpCode)
        {
            if (_otpStorage.TryGetValue(GetKey(email, otpCode), out var otp))
            {
                otp.IsUsed = true;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> MarkOtpAsVerifiedAsync(string email, string otpCode)
        {
            if (_otpStorage.TryGetValue(GetKey(email, otpCode), out var otp))
            {
                otp.VerifiedAt = DateTime.UtcNow;
                otp.AttemptsCount++;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        private string GetKey(string email, string otp) => $"{email}:{otp}";

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
