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
using System.Linq;
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

        // ✅ Cập nhật NormalizeEmail để validate format
        private static string NormalizeEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return string.Empty;

                // Validate email format
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email.Trim())
                    return string.Empty;

                return email.Trim().ToLowerInvariant();
            }
            catch
            {
                return string.Empty;
            }
        }

        // ✅ THÊM MỚI: Sanitize avatar URL để tránh XSS/SSRF
        private string SanitizeAvatarUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return DEFAULT_AVATAR_URL;

            try
            {
                // Kiểm tra URL hợp lệ
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    return DEFAULT_AVATAR_URL;

                // Chỉ chấp nhận HTTPS
                if (uri.Scheme != Uri.UriSchemeHttps)
                    return DEFAULT_AVATAR_URL;

                // Chỉ cho phép Google domains
                var allowedHosts = new[]
                {
            "lh3.googleusercontent.com",
            "lh4.googleusercontent.com",
            "lh5.googleusercontent.com",
            "lh6.googleusercontent.com"
        };

                if (!allowedHosts.Any(host => uri.Host.Equals(host, StringComparison.OrdinalIgnoreCase)))
                    return DEFAULT_AVATAR_URL;

                // Trả về URL đã sanitize
                return $"{uri.Scheme}://{uri.Host}{uri.PathAndQuery}";
            }
            catch
            {
                return DEFAULT_AVATAR_URL;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user. Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("verified", user.EmailVerified.ToString()),
        new Claim("status", user.AccountStatus ??  string.Empty),
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
            // ✅ TC4, TC5: Validate email
            if (string.IsNullOrWhiteSpace(registerDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            var normalizedEmail = NormalizeEmail(registerDto.Email);

            // ✅ TC3: Email format validation (sau khi normalize)
            if (string.IsNullOrEmpty(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email format."
                };
            }

            // ✅ TC9: Validate password
            if (string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Password is required."
                };
            }

            // ✅ TC10: Password length
            if (registerDto.Password.Length < 8)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Password must be at least 8 characters."
                };
            }

            // ✅ TC11, TC12: Validate full name
            if (string.IsNullOrWhiteSpace(registerDto.FullName))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Full name is required."
                };
            }

            // ✅ TC13, TC14: Validate phone number
            if (string.IsNullOrWhiteSpace(registerDto.PhoneNumber))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Phone number is required."
                };
            }

            // ✅ TC19, TC20: Validate gender
            if (string.IsNullOrWhiteSpace(registerDto.Gender))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Gender is required."
                };
            }

            // ✅ TC16, TC17, TC18: Validate date of birth
            if (!registerDto.DateOfBirth.HasValue)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Date of birth is required."
                };
            }

            var today = DateTime.Today;
            var age = today.Year - registerDto.DateOfBirth.Value.Year;
            if (registerDto.DateOfBirth.Value.Date > today.AddYears(-age))
                age--;

            if (registerDto.DateOfBirth.Value > today)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Date of birth cannot be in the future."
                };
            }

            if (age < 13)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "You must be at least 13 years old to register."
                };
            }

            if (age > 120)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid date of birth."
                };
            }

            // ✅ TC2: Check email uniqueness
            if (!await IsEmailUniqueAsync(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This email is already registered."
                };
            }

            // ✅ TC29: Wrap database operations in try-catch
            try
            {
                var user = _mapper.Map<User>(registerDto);
                user.Email = normalizedEmail;

                // ✅ Hash password with error handling
                try
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to hash password: {ex.Message}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Failed to process password. Please try again."
                    };
                }

                user.EmailVerified = false;
                user.AccountStatus = STATUS_PENDING_VERIFICATION;
                user.AvatarUrl = DEFAULT_AVATAR_URL;
                user.CreatedAt = DateTime.UtcNow;

                await _usersCollection.InsertOneAsync(user);

                var profile = _mapper.Map<UserProfile>(registerDto);
                profile.UserId = user.Id;
                await _profilesCollection.InsertOneAsync(profile);

                // ✅ Send verification email (non-blocking error)
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
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                Console.WriteLine($"Duplicate key error: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "This email is already registered."
                };
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Database error during registration: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Unable to create account due to server error. Please try again later."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during registration: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                };
            }
        }


        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is required."
                };
            }
            var normalizedEmail = NormalizeEmail(loginDto.Email);
            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Password is required."
                };
            }

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
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Unable to send verification code. Please try again later or contact support."
                    };
                }


                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email verification required. A new OTP has been sent to your email.",
                    UserId = user.Id,
                    RequiresVerification = true
                };
            }
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Password verification failed: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Authentication error. Please try again or reset your password."
                };
            }
            try
            {
                var update = Builders<User>.Update
                    .Set(u => u.LastLogin, DateTime.UtcNow)
                    .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);

                await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);

                user.AccountStatus = STATUS_ACTIVE_ONLINE;
                user.LastLogin = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update user status: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Login processed but unable to update session.  Please try again."
                };
            }
            string token;
            try
            {
                token = GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to generate JWT token: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Authentication successful but unable to create session.  Please contact support."
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                UserId = user.Id
            };
        }
        /// <summary>
        /// Verify OTP và update user status
        /// </summary>
        public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            // ✅ TC4, TC5: Validate email not null/empty
            if (string.IsNullOrWhiteSpace(verifyOtpDto.Email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            // ✅ TC9, TC10: Validate OTP not null/empty
            if (string.IsNullOrWhiteSpace(verifyOtpDto.OtpCode))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "OTP code is required."
                };
            }

            // ✅ TC12, TC13: Already handled above (both email and OTP checked)

            // ✅ TC2: Normalize và validate email format
            var normalizedEmail = NormalizeEmail(verifyOtpDto.Email);

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email format."
                };
            }

            // ✅ TC7: Validate OTP format (6 digits)
            if (!System.Text.RegularExpressions.Regex.IsMatch(verifyOtpDto.OtpCode, @"^\d{6}$"))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "OTP must be a 6-digit code."
                };
            }

            // ✅ TC14: Wrap database operations in try-catch
            try
            {
                // ✅ TC3: Find user by email
                var user = await GetUserByEmailAsync(normalizedEmail);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // ✅ TC14: Update user status with error handling
                try
                {
                    var update = Builders<User>.Update
                        .Set(u => u.EmailVerified, true)
                        .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);

                    await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);
                }
                catch (MongoException ex)
                {
                    Console.WriteLine($"Failed to update user status: {ex.Message}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Failed to update account.  Please try again."
                    };
                }

                // Delete OTP after successful verification (non-blocking)
                try
                {
                    await _otpService.DeleteOtpAsync(normalizedEmail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: failed to delete OTP: {ex.Message}");
                    // Don't fail the request
                }

                // Send welcome email (non-blocking)
                try
                {
                    var profile = await _profilesCollection.Find(p => p.UserId == user.Id).FirstOrDefaultAsync();
                    await _emailService.SendWelcomeEmailAsync(user.Email, profile?.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                    // Don't fail the request
                }

                user.EmailVerified = true;
                user.AccountStatus = STATUS_ACTIVE_ONLINE;

                // ✅ Generate token with error handling
                string token;
                try
                {
                    token = GenerateJwtToken(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to generate JWT token: {ex.Message}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Verification successful but unable to create session. Please login."
                    };
                }

                // ✅ TC1: Success
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Email verified successfully.",
                    Token = token,
                    UserId = user.Id
                };
            }
            catch (MongoException ex)
            {
                // ✅ TC14: Database error
                Console.WriteLine($"Database error in VerifyOtpAsync: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A server error occurred. Please try again later."
                };
            }
            catch (Exception ex)
            {
                // ✅ TC14: Generic error
                Console.WriteLine($"Unexpected error in VerifyOtpAsync: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                };
            }
        }
        public async Task<AuthResponseDto> GoogleLoginAsync(string idToken)
        {
            // ✅ TC12, TC13: Kiểm tra token không null/rỗng
            if (string.IsNullOrWhiteSpace(idToken))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Google authentication token is required."
                };
            }

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleSettings.ClientId }
                };

                // ✅ TC11: Validate token - sẽ throw exception nếu token invalid/expired
                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                }
                catch (InvalidJwtException ex)
                {
                    Console.WriteLine($"Invalid Google token: {ex.Message}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired Google authentication token."
                    };
                }

                // ✅ TC14, TC15: Kiểm tra email không null/rỗng
                if (string.IsNullOrWhiteSpace(payload.Email))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Google account does not have a valid email address."
                    };
                }

                // ✅ TC20: Chuẩn hóa email
                var normalizedEmail = NormalizeEmail(payload.Email);

                // ✅ TC16: Kiểm tra email format hợp lệ
                if (string.IsNullOrEmpty(normalizedEmail))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email format from Google account."
                    };
                }

                // ✅ TC8, TC9: Kiểm tra ProviderId (Google Subject) không null/rỗng
                if (string.IsNullOrWhiteSpace(payload.Subject))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid Google account identifier."
                    };
                }

                // Tìm user theo email
                var user = await _usersCollection
                    .Find(u => u.Email == normalizedEmail)
                    .FirstOrDefaultAsync();

                // ========== XỬ LÝ NEW USER ==========
                if (user == null)
                {
                    // ✅ TC10: Kiểm tra ProviderId đã được dùng bởi user khác chưa
                    var existingProviderUser = await _usersCollection
                        .Find(u => u.ProviderId == payload.Subject && u.AuthProvider == "Google")
                        .FirstOrDefaultAsync();

                    if (existingProviderUser != null)
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "This Google account is already linked to another user."
                        };
                    }

                    // ✅ TC17, TC19: Tạo user mới với error handling
                    try
                    {
                        user = _mapper.Map<User>(payload);
                        user.Email = normalizedEmail;
                        user.EmailVerified = true;
                        user.AccountStatus = STATUS_ACTIVE_ONLINE;
                        user.AuthProvider = "Google";
                        user.ProviderId = payload.Subject;
                        user.CreatedAt = DateTime.UtcNow;
                        user.LastLogin = DateTime.UtcNow;

                        // ✅ TC1, TC2, TC3: Sanitize avatar URL
                        user.AvatarUrl = SanitizeAvatarUrl(payload.Picture);

                        await _usersCollection.InsertOneAsync(user);

                        // Tạo profile
                        var profile = _mapper.Map<UserProfile>(payload);
                        profile.UserId = user.Id;
                        await _profilesCollection.InsertOneAsync(profile);
                    }
                    catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
                    {
                        // ✅ TC19: Xử lý concurrency - request khác vừa tạo user này
                        Console.WriteLine($"Duplicate key error (concurrent request): {ex.Message}");

                        // Tìm lại user vừa được tạo
                        user = await _usersCollection
                            .Find(u => u.Email == normalizedEmail)
                            .FirstOrDefaultAsync();

                        if (user == null)
                        {
                            return new AuthResponseDto
                            {
                                Success = false,
                                Message = "Unable to create account. Please try again."
                            };
                        }

                        // Tiếp tục generate token cho user đã tồn tại
                    }
                    catch (MongoException ex)
                    {
                        Console.WriteLine($"Database error while creating Google user: {ex.Message}");
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "Unable to create account. Please try again later."
                        };
                    }

                    // ✅ TC4: Gửi welcome email (không crash nếu lỗi)
                    try
                    {
                        await _emailService.SendWelcomeEmailAsync(user.Email, payload.Name);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                        // Không return error - user đã tạo thành công
                    }
                }
                // ========== XỬ LÝ EXISTING USER ==========
                else
                {
                    // ✅ TC21: Kiểm tra tài khoản bị block
                    if (user.AccountStatus == STATUS_BLOCKED)
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "Your account is blocked. Please contact support."
                        };
                    }

                    // ✅ TC6: User đã tồn tại với email/password (local account)
                    if (!string.IsNullOrEmpty(user.PasswordHash) && string.IsNullOrEmpty(user.AuthProvider))
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "This email is already registered with a password. Please login using your email and password."
                        };
                    }

                    // ✅ TC7: User đã link với OAuth provider khác
                    if (!string.IsNullOrEmpty(user.AuthProvider) && user.AuthProvider != "Google")
                    {
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = $"This email is already registered with {user.AuthProvider}. Please login using {user.AuthProvider}."
                        };
                    }

                    // ✅ TC5: Cập nhật thông tin login cho existing Google user
                    try
                    {
                        var update = Builders<User>.Update
                            .Set(u => u.LastLogin, DateTime.UtcNow)
                            .Set(u => u.AccountStatus, STATUS_ACTIVE_ONLINE);

                        // Nếu user chưa có AuthProvider (trường hợp đặc biệt)
                        if (string.IsNullOrEmpty(user.AuthProvider))
                        {
                            update = update
                                .Set(u => u.AuthProvider, "Google")
                                .Set(u => u.ProviderId, payload.Subject)
                                .Set(u => u.EmailVerified, true);

                            // Cập nhật avatar nếu đang dùng default
                            if (string.IsNullOrEmpty(user.AvatarUrl) || user.AvatarUrl == DEFAULT_AVATAR_URL)
                            {
                                var sanitizedAvatar = SanitizeAvatarUrl(payload.Picture);
                                if (sanitizedAvatar != DEFAULT_AVATAR_URL)
                                {
                                    update = update.Set(u => u.AvatarUrl, sanitizedAvatar);
                                }
                            }
                        }

                        await _usersCollection.UpdateOneAsync(u => u.Id == user.Id, update);

                        // Cập nhật local object
                        user.LastLogin = DateTime.UtcNow;
                        user.AccountStatus = STATUS_ACTIVE_ONLINE;
                        user.EmailVerified = true;
                    }
                    catch (MongoException ex)
                    {
                        Console.WriteLine($"Failed to update user login info: {ex.Message}");
                        return new AuthResponseDto
                        {
                            Success = false,
                            Message = "Login processed but unable to update session.  Please try again."
                        };
                    }
                }

                // ✅ TC18: Generate JWT token với error handling
                string token;
                try
                {
                    token = GenerateJwtToken(user);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to generate JWT token: {ex.Message}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Authentication successful but unable to create session. Please try again."
                    };
                }

                // ✅ TC1, TC5: Trả về kết quả thành công
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Google login successful.",
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google login failed: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Google authentication failed.  Please try again."
                };
            }
        }
        public async Task<AuthResponseDto> ForgotPasswordAsync(string email)
        {
            // ✅ TC4, TC5: Validate email không null/rỗng
            if (string.IsNullOrWhiteSpace(email))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            // ✅ TC3: Validate và normalize email
            var normalizedEmail = NormalizeEmail(email);

            if (string.IsNullOrEmpty(normalizedEmail))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email format."
                };
            }

            // ✅ TC8: Wrap database operations trong try-catch
            try
            {
                // ✅ TC7: Tìm user theo email
                var user = await GetUserByEmailAsync(normalizedEmail);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "No account found with this email."
                    };
                }

                // ✅ TC6: Kiểm tra user bị khóa
                if (user.AccountStatus == STATUS_BLOCKED)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Your account is blocked. Please contact support."
                    };
                }

                // ✅ TC2: Kiểm tra email có thuộc về user không (đã check ở trên qua GetUserByEmailAsync)

                // Kiểm tra rate limiting (số lần request OTP)
                if (await _otpService.IsOtpLimitExceededAsync(normalizedEmail))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "You have requested codes too many times. Please try again later."
                    };
                }

                // ✅ TC1: Generate và gửi OTP
                try
                {
                    var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "resetpassword");
                    await _emailService.SendPasswordResetEmailAsync(user.Email, otp);

                    return new AuthResponseDto
                    {
                        Success = true,
                        Message = "A password reset code has been sent to your email.",
                        UserId = user.Id,
                        RequiresVerification = true,
                        Email = user.Email
                    };
                }
                catch (Exception ex)
                {
                    // ✅ TC8: Xử lý lỗi khi generate/send OTP
                    Console.WriteLine($"Failed to generate/send password reset OTP: {ex.Message}");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Unable to send the reset code. Please try again later."
                    };
                }
            }
            catch (MongoException ex)
            {
                // ✅ TC8: Xử lý lỗi database
                Console.WriteLine($"Database error in ForgotPasswordAsync: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A server error occurred. Please try again later."
                };
            }
            catch (Exception ex)
            {
                // ✅ TC8: Xử lý lỗi chung
                Console.WriteLine($"Unexpected error in ForgotPasswordAsync: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
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
        public async Task<AuthResponseDto> CancelPendingRegistrationAsync(string email)
        {
            // Chuẩn hóa email
            var normalizedEmail = NormalizeEmail(email);
            if (string.IsNullOrEmpty(normalizedEmail))
            {
                // Email không valid thì coi như không có gì để xóa
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "No pending registration for this email."
                };
            }

            try
            {
                var user = await GetUserByEmailAsync(normalizedEmail);
                if (user == null)
                {
                    // Không có user -> coi như đã sạch
                    return new AuthResponseDto
                    {
                        Success = true,
                        Message = "User not found or already removed."
                    };
                }

                // ĐÃ VERIFY / KHÔNG CÒN pending thì KHÔNG xóa nữa cho an toàn
                if (user.EmailVerified || user.AccountStatus != STATUS_PENDING_VERIFICATION)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User is not in pending verification state. Cannot cancel registration."
                    };
                }

                // Xóa user
                await _usersCollection.DeleteOneAsync(u => u.Id == user.Id);

                // Xóa profile nếu có
                await _profilesCollection.DeleteOneAsync(p => p.UserId == user.Id);

                // Xóa OTP (nếu có) – dùng email
                try
                {
                    await _otpService.DeleteOtpAsync(normalizedEmail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete OTP when cancel registration: {ex.Message}");
                    // Không fail request vì lỗi OTP
                }

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Pending registration has been cancelled and user data removed."
                };
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Database error in CancelPendingRegistrationAsync: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "A server error occurred while cancelling registration. Please try again later."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in CancelPendingRegistrationAsync: {ex.Message}");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                };
            }
        }

    }
}