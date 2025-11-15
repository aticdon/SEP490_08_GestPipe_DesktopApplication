using GestPipePowerPonit.Models.DTOs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Services
{
    public class AuthService
    {
        private readonly ApiService _apiService;
        private readonly string _googleClientId;
        private readonly string _googleClientSecret;

        // ✅ Constants for file paths
        private const string TOKEN_FILE_PATH = "token.json";
        private static string TOKEN_FOLDER => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GestPipeTokenStore"
        );
        private static string TOKEN_FILE_FULL_PATH => Path.Combine(TOKEN_FOLDER, TOKEN_FILE_PATH);

        public AuthService(string googleClientId = "", string googleClientSecret = "")
        {
            _apiService = new ApiService();

            // ✅ Lấy từ App.config nếu không truyền vào
            if (string.IsNullOrEmpty(googleClientId))
            {
                googleClientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"] ?? "";
            }
            if (string.IsNullOrEmpty(googleClientSecret))
            {
                googleClientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"] ?? "";
            }

            _googleClientId = googleClientId;
            _googleClientSecret = googleClientSecret;

            Console.WriteLine($"[AuthService] Initialized with GoogleClientId: {(_googleClientId != "" ? "✅ SET" : "❌ NOT SET")}");
        }

        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            return await _apiService.PostAsync<AuthResponseDto>("auth/login", loginDto);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            return await _apiService.PostAsync<AuthResponseDto>("auth/register", registerDto);
        }

        public async Task<AuthResponseDto> ValidateOtpAsync(string email, string otpCode, string purpose = "registration")
        {
            var verifyDto = new VerifyOtpDto
            {
                Email = email,
                OtpCode = otpCode
            };

            string endpoint = $"auth/validate-otp?purpose={purpose}";
            return await _apiService.PostAsync<AuthResponseDto>(endpoint, verifyDto);
        }

        public async Task<AuthResponseDto> ForgotPasswordAsync(string email)
        {
            var emailDto = new EmailRequestDto { Email = email };
            return await _apiService.PostAsync<AuthResponseDto>("auth/forgot-password", emailDto);
        }

        public async Task<AuthResponseDto> ResendOtpAsync(string email)
        {
            var emailDto = new EmailRequestDto { Email = email };
            return await _apiService.PostAsync<AuthResponseDto>("auth/resend-otp", emailDto);
        }

        public async Task<AuthResponseDto> ResendResetOtpAsync(string email)
        {
            var emailDto = new EmailRequestDto { Email = email };
            return await _apiService.PostAsync<AuthResponseDto>("auth/resend-reset-otp", emailDto);
        }

        public async Task<AuthResponseDto> ResetPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            var resetDto = new ResetPasswordDto
            {
                Email = email,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword
            };

            return await _apiService.PostAsync<AuthResponseDto>("auth/reset-password", resetDto);
        }

        /// <summary>
        /// ✅ FIXED: Lấy Google ID Token - Sửa để hoạt động trên Desktop
        /// </summary>
        public async Task<string?> GetGoogleIdTokenAsync(string clientId = null, string clientSecret = null, string userIdentifier = "user")
        {
            try
            {
                var cid = clientId ?? _googleClientId;
                var csecret = clientSecret ?? _googleClientSecret;

                if (string.IsNullOrWhiteSpace(cid))
                {
                    Console.WriteLine("❌ [GetGoogleIdTokenAsync] Google ClientId not configured");
                    throw new InvalidOperationException("Google ClientId is not configured.");
                }

                Console.WriteLine($"\n[GetGoogleIdTokenAsync] Starting OAuth flow...");
                Console.WriteLine($"  ClientId: {cid.Substring(0, Math.Min(20, cid.Length))}...");

                var clientSecrets = new ClientSecrets
                {
                    ClientId = cid,
                    ClientSecret = csecret ?? string.Empty
                };

                // ✅ Scopes
                string[] scopes = new[] {
            "openid",
            "email",
            "profile",
            "https://www.googleapis.com/auth/userinfo.profile"
        };

                // ✅ Token storage
                if (!Directory.Exists(TOKEN_FOLDER))
                {
                    Directory.CreateDirectory(TOKEN_FOLDER);
                }

                var dataStore = new FileDataStore(TOKEN_FOLDER, true);

                Console.WriteLine($"  📁 Token path: {TOKEN_FOLDER}");
                Console.WriteLine($"  🔐 Authorizing with Google...");

                // ✅ SỬA: Không dùng custom receiver, để Google tự động xử lý
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    scopes,
                    userIdentifier,
                    CancellationToken.None,
                    dataStore
                // ✅ Không truyền receiver parameter - để Google dùng default
                );

                Console.WriteLine($"  ✅ Authorization successful");

                if (credential?.Token != null)
                {
                    Console.WriteLine($"  📊 Token info:");
                    Console.WriteLine($"    - TokenType: {credential.Token.TokenType}");
                    Console.WriteLine($"    - IdToken: {(credential.Token.IdToken != null ? "✅ Available" : "❌ NULL")}");
                }

                var idToken = credential?.Token?.IdToken;

                // Fallback to AccessToken if IdToken not available
                if (string.IsNullOrEmpty(idToken) && credential?.Token?.AccessToken != null)
                {
                    Console.WriteLine($"  ⚠️  IdToken not available, using AccessToken as fallback");
                    idToken = credential.Token.AccessToken;
                }

                if (string.IsNullOrEmpty(idToken))
                {
                    Console.WriteLine("❌ [GetGoogleIdTokenAsync] No token available");
                    return null;
                }

                Console.WriteLine($"  ✅ Token retrieved successfully\n");
                return idToken;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("❌ [GetGoogleIdTokenAsync] User canceled authorization");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GetGoogleIdTokenAsync] Error: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   InnerException: {ex.InnerException.Message}");
                }
                return null;
            }
        }

        /// <summary>
        /// Đăng nhập / đăng ký bằng Google
        /// </summary>
        public async Task<AuthResponseDto> GoogleLoginAsync(string clientId = null, string clientSecret = null)
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[GoogleLoginAsync] STARTED");
                Console.WriteLine(new string('=', 60));

                // ✅ Lấy ID Token từ Google
                var idToken = await GetGoogleIdTokenAsync(clientId, clientSecret);

                if (string.IsNullOrEmpty(idToken))
                {
                    Console.WriteLine("❌ [GoogleLoginAsync] Failed to get idToken");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Không thể lấy Google id_token. Kiểm tra cấu hình Google ClientId hoặc người dùng đã hủy quá trình xác thực."
                    };
                }

                Console.WriteLine("[GoogleLoginAsync] 📤 Sending token to backend...");

                var googleDto = new GoogleLoginDto
                {
                    IdToken = idToken
                };

                // ✅ Gửi lên backend
                var response = await _apiService.PostAsync<AuthResponseDto>("auth/google-login", googleDto);

                Console.WriteLine($"[GoogleLoginAsync] ✅ Backend response received");
                Console.WriteLine($"  Success: {response?.Success}");
                Console.WriteLine($"  Message: {response?.Message}");
                Console.WriteLine($"  RequiresProfileCompletion: {response?.RequiresProfileCompletion}");

                if (response?.Success == true)
                {
                    Console.WriteLine($"  Token: {(response.Token != null ? response.Token.Substring(0, 20) + "..." : "NULL")}");
                    Console.WriteLine($"  UserId: {response.UserId}");
                    Console.WriteLine($"  Email: {response.Email}");
                }

                Console.WriteLine(new string('=', 60) + "\n");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [GoogleLoginAsync] Exception: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                Console.WriteLine(new string('=', 60) + "\n");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public void SaveUserSession(AuthResponseDto response, bool rememberMe)
        {
            if (response?.Token != null && response?.UserId != null)
            {
                Properties.Settings.Default.AuthToken = response.Token;
                Properties.Settings.Default.UserId = response.UserId;

                if (!string.IsNullOrEmpty(response.Email))
                {
                    Properties.Settings.Default.UserEmail = response.Email;
                }

                // ✅ Lưu giá trị rememberMe thật sự
                Properties.Settings.Default.RememberMe = rememberMe;
                Properties.Settings.Default.Save();

                _apiService.SetAuthToken(response.Token);
            }
        }

        public void ClearUserSession()
        {
            Properties.Settings.Default.AuthToken = string.Empty;
            Properties.Settings.Default.UserId = string.Empty;
            Properties.Settings.Default.UserEmail = string.Empty;

            // ✅ KHÔNG XÓA SavedEmail và RememberMe khi logout
            // Chỉ xóa khi user uncheck "Remember Me" trong LoginForm

            Properties.Settings.Default.Save();

            _apiService.ClearAuthToken();
        }

        /// <summary>
        /// ✅ LOGOUT - Clear all stored data
        /// </summary>
        public async Task<AuthResponseDto> LogoutAsync()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[LogoutAsync] STARTED");
                Console.WriteLine(new string('=', 60));

                // 1. Call backend logout API
                var token = GetAuthToken();
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        Console.WriteLine("[Logout] 📤 Calling backend logout API...");
                        var response = await _apiService.PostAsync<AuthResponseDto>("auth/logout", new { });
                        Console.WriteLine($"[Logout] ✅ Backend response: {response?.Success}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Logout] ⚠️ Backend API error (ignored): {ex.Message}");
                    }
                }

                // 2. ✅ Clear local token file
                Console.WriteLine("[Logout] 🗑️ Clearing token files...");
                ClearStoredTokenFile();

                // 3. ✅ Clear Properties.Settings (AuthToken, UserId, UserEmail)
                Console.WriteLine("[Logout] 🗑️ Clearing user session...");
                ClearUserSession();

                Console.WriteLine("[Logout] ✅ Logout completed successfully");
                Console.WriteLine(new string('=', 60) + "\n");

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Logged out successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [LogoutAsync] Error: {ex.Message}");
                Console.WriteLine(new string('=', 60) + "\n");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Logout failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// ✅ Clear stored token file (GestPipeTokenStore folder)
        /// </summary>
        private void ClearStoredTokenFile()
        {
            try
            {
                Console.WriteLine($"[ClearToken] Token folder: {TOKEN_FOLDER}");

                // Delete entire folder and all its contents
                if (Directory.Exists(TOKEN_FOLDER))
                {
                    // List files before deletion
                    var files = Directory.GetFiles(TOKEN_FOLDER);
                    Console.WriteLine($"[ClearToken] Found {files.Length} file(s):");
                    foreach (var file in files)
                    {
                        Console.WriteLine($"  - {Path.GetFileName(file)}");
                    }

                    // Delete folder recursively
                    Directory.Delete(TOKEN_FOLDER, recursive: true);
                    Console.WriteLine($"[ClearToken] ✅ Deleted folder: {TOKEN_FOLDER}");
                }
                else
                {
                    Console.WriteLine($"[ClearToken] ℹ️ Folder does not exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ClearToken] ⚠️ Error: {ex.Message}");
            }
        }

        public bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(Properties.Settings.Default.AuthToken);
        }

        public string GetAuthToken()
        {
            return Properties.Settings.Default.AuthToken ?? string.Empty;
        }

        public string GetUserId()
        {
            return Properties.Settings.Default.UserId ?? string.Empty;
        }

        public string GetUserEmail()
        {
            return Properties.Settings.Default.UserEmail ?? string.Empty;
        }
    }
}