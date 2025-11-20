using AutoMapper;
using GestPipe.Backend.Models;
using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Models.DTOs.Auth;
using GestPipe.Backend.Models.Setting;
using GestPipe.Backend.Services.IServices;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GestPipe.Backend.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private const string DEFAULT_AVATAR_URL =
            "https://www.google.com/url?sa=i&url=https%3A%2F%2Fhaxxcelsolutions.com%2F&psig=AOvVaw0rXB7bKtfTDPL-LoXqGHGu&ust=1763642789945000&source=images&cd=vfe&opi=89978449&ved=0CBEQjRxqFwoTCPDYzMSf_pADFQAAAAAdAAAAABAE";


        // ✅ Đồng bộ status bằng const để tránh typo
        private const string STATUS_PENDING_VERIFICATION = "pending_verification";
        private const string STATUS_ACTIVE_ONLINE = "activeonline";
        private const string STATUS_ACTIVE_OFFLINE = "activeoffline";
        private const string STATUS_BLOCKED = "blocked";


        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<UserProfile> _profilesCollection;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly JwtSettings _jwtSettings;
        private readonly GoogleSettings _googleSettings;
        private readonly IMapper _mapper;


        public AuthService(
            IMongoClient mongoClient,
            IOptions<MongoDbSettings> dbSettings,
            IOptions<JwtSettings> jwtSettings,
            IOptions<GoogleSettings> googleSettings,
            IEmailService emailService,
            IOtpService otpService,
            IMapper mapper)
        {
            var database = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _usersCollection = database.GetCollection<User>("Users");
            _profilesCollection = database.GetCollection<UserProfile>("User_Profiles");
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _googleSettings = googleSettings?.Value ?? throw new ArgumentNullException(nameof(googleSettings));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        #region Helpers


        private static string NormalizeEmail(string email)
        {
            return email?.Trim().ToLowerInvariant();
        }


        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("verified", user.EmailVerified.ToString()),
                new Claim("status", user.AccountStatus ?? string.Empty),
            };


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        #endregion


        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var normalizedEmail = NormalizeEmail(registerDto.Email);


            if (!await IsEmailUniqueAsync(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This email is already registered."
                };
            }


            var user = _mapper.Map<User>(registerDto);
            user.Email = normalizedEmail;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);


            // ✅ Default trạng thái sau khi đăng ký
            user.EmailVerified = false;
            user.AccountStatus = STATUS_PENDING_VERIFICATION;
            user.AvatarUrl = DEFAULT_AVATAR_URL;


            await _usersCollection.InsertOneAsync(user);


            var profile = _mapper.Map<UserProfile>(registerDto);
            profile.UserId = user.Id;
            await _profilesCollection.InsertOneAsync(profile);


            try
            {
                var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "registration");
                await _emailService.SendVerificationEmailAsync(user.Email, otp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send verification email: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Account created, but we couldn't send the verification code. Please request a new OTP.",
                    UserId = user.Id,
                    RequiresVerification = true
                };
            }


            return new AuthResponseDto
            {
                Success = true,
                Message = "Registration successful. Please check your email for the verification code.",
                UserId = user.Id,
                RequiresVerification = true
            };
        }


        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var normalizedEmail = NormalizeEmail(loginDto.Email);


            var user = await GetUserByEmailAsync(normalizedEmail);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            if (!string.IsNullOrEmpty(user.AuthProvider)
                 && user.AuthProvider == "Google"
                 && string.IsNullOrEmpty(user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Your account was created using Google. Please sign in with Google."
                };
            }
            if (user.AccountStatus == STATUS_BLOCKED)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Your account is blocked. Please contact support."
                };
            }


            if (!user.EmailVerified)
            {
                try
                {
                    var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "registration");
                    await _emailService.SendVerificationEmailAsync(user.Email, otp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send verification email: {ex.Message}");
                }


                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email verification required. A new OTP has been sent to your email.",
                    UserId = user.Id,
                    RequiresVerification = true
                };
            }


            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }


            var update = Builders<User>.Update
                .Set(u => u.LastLogin, DateTime.UtcNow)
                .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);


            await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);


            user.AccountStatus = STATUS_ACTIVE_ONLINE;
            user.LastLogin = DateTime.UtcNow;


            var token = GenerateJwtToken(user);


            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                UserId = user.Id
            };
        }


        /// <summary>
        /// OTP đã validate ở Controller. Hàm này chỉ cập nhật user & gửi mail.
        /// </summary>
        public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var normalizedEmail = NormalizeEmail(verifyOtpDto.Email);


            var user = await GetUserByEmailAsync(normalizedEmail);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User not found."
                };
            }


            var update = Builders<User>.Update
                .Set(u => u.EmailVerified, true)
                .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);


            await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);


            // Xóa OTP sau khi xác thực thành công
            try
            {
                await _otpService.DeleteOtpAsync(normalizedEmail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: failed to delete OTP: {ex.Message}");
            }


            try
            {
                var profile = await _profilesCollection.Find(p => p.UserId == user.Id).FirstOrDefaultAsync();
                await _emailService.SendWelcomeEmailAsync(user.Email, profile?.FullName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send welcome email: {ex.Message}");
            }


            user.EmailVerified = true;
            user.AccountStatus = STATUS_ACTIVE_ONLINE;


            var token = GenerateJwtToken(user);


            return new AuthResponseDto
            {
                Success = true,
                Message = "Email verified successfully.",
                Token = token,
                UserId = user.Id
            };
        }


        public async Task<AuthResponseDto> GoogleLoginAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleSettings.ClientId }
                };


                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                var normalizedEmail = NormalizeEmail(payload.Email);


                var user = await _usersCollection
                    .Find(u => u.Email == normalizedEmail)
                    .FirstOrDefaultAsync();


                if (user == null)
                {
                    user = _mapper.Map<User>(payload);
                    user.Email = normalizedEmail;
                    user.EmailVerified = true;
                    user.AccountStatus = STATUS_ACTIVE_ONLINE;
                    user.AuthProvider = "Google";
                    user.ProviderId = payload.Subject;


                    user.AvatarUrl = !string.IsNullOrEmpty(payload.Picture)
                        ? payload.Picture
                        : DEFAULT_AVATAR_URL;


                    await _usersCollection.InsertOneAsync(user);


                    var profile = _mapper.Map<UserProfile>(payload);
                    profile.UserId = user.Id;
                    await _profilesCollection.InsertOneAsync(profile);


                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(user.Email, payload.Name);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                    }
                }
                else
                {
                    var update = Builders<User>.Update
                        .Set(u => u.LastLogin, DateTime.UtcNow)
                        .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);


                    if (string.IsNullOrEmpty(user.AuthProvider))
                    {
                        update = update
                            .Set(u => u.AuthProvider, "Google")
                            .Set(u => u.ProviderId, payload.Subject)
                            .Set(u => u.EmailVerified, true);


                        if (string.IsNullOrEmpty(user.AvatarUrl) || user.AvatarUrl == DEFAULT_AVATAR_URL)
                        {
                            if (!string.IsNullOrEmpty(payload.Picture))
                            {
                                update = update.Set(u => u.AvatarUrl, payload.Picture);
                            }
                        }
                    }


                    await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);


                    user.LastLogin = DateTime.UtcNow;
                    user.AccountStatus = STATUS_ACTIVE_ONLINE;
                    user.EmailVerified = true;
                }


                var token = GenerateJwtToken(user);


                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Google login successful.",
                    Token = token,
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google login failed: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Google authentication failed: {ex.Message}"
                };
            }
        }


        public async Task<AuthResponseDto> ForgotPasswordAsync(string email)
        {
            var normalizedEmail = NormalizeEmail(email);


            var user = await GetUserByEmailAsync(normalizedEmail);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "No account found with this email."
                };
            }


            if (await _otpService.IsOtpLimitExceededAsync(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "You have requested codes too many times. Please try again later."
                };
            }


            try
            {
                var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "resetpassword");
                await _emailService.SendPasswordResetEmailAsync(user.Email, otp);


                return new AuthResponseDto
                {
                    Success = true,
                    Message = "A password reset code has been sent to your email.",
                    UserId = user.Id,
                    RequiresVerification = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to generate/send password reset OTP: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Unable to send the reset code. Please try again later."
                };
            }
        }


        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetDto)
        {
            var normalizedEmail = NormalizeEmail(resetDto.Email);


            var user = await GetUserByEmailAsync(normalizedEmail);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }


            try
            {
                var newHash = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);


                var update = Builders<User>.Update
                    .Set(u => u.PasswordHash, newHash)
                    .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);


                await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);


                try
                {
                    await _emailService.SendPasswordResetConfirmationEmailAsync(user.Email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send password reset confirmation email: {ex.Message}");
                }


                user.AccountStatus = STATUS_ACTIVE_ONLINE;


                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Password has been reset successfully. You can now sign in with your new password.",
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to reset password: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Unable to reset password. Please try again later."
                };
            }
        }


        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var normalizedEmail = NormalizeEmail(email);
            var existingUser = await _usersCollection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
            return existingUser == null;
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            var normalizedEmail = NormalizeEmail(email);
            return await _usersCollection.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
        }


        public async Task<User> GetUserByIdAsync(string userId)
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return null;
            }


            return await _usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
        }


        public async Task<AuthResponseDto> LogoutAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID."
                    };
                }


                var user = await GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }


                var update = Builders<User>.Update
                    .Set(u => u.AccountStatus, STATUS_ACTIVE_OFFLINE);


                await _usersCollection.UpdateOneAsync(u => u.Id == userId, update);


                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Logged out successfully.",
                    UserId = userId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to logout user: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Unable to log out. Please try again later."
                };
            }
        }
    }
}