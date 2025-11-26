using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Models.DTOs.Auth;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;


namespace GestPipe.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IGestureInitializationService _gestureInitService;
        private readonly ILogger<AuthController> _logger;


        public AuthController(
            IAuthService authService,
            IOtpService otpService,
            IEmailService emailService,
            IGestureInitializationService gestureInitService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _otpService = otpService;
            _emailService = emailService;
            _gestureInitService = gestureInitService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                _logger.LogInformation("Đăng ký tài khoản mới: Email={Email}, FullName={FullName}",
                    registerDto.Email, registerDto.FullName);


                var response = await _authService.RegisterAsync(registerDto);


                if (!response.Success)
                    return BadRequest(response);


                // ✅ Initialize default gesture folder async (fire and forget)
                if (!string.IsNullOrEmpty(response.UserId))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _gestureInitService.CreateUserDataFolderAsync(response.UserId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to initialize gestures for new user: {UserId}", response.UserId);
                        }
                    });
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký tài khoản: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi đăng ký tài khoản. Vui lòng thử lại sau."
                });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();


                _logger.LogInformation("Đăng nhập: Email={Email}, IP={IP}, UA={UA}",
                    loginDto.Email, ipAddress, userAgent);


                var response = await _authService.LoginAsync(loginDto);


                if (!response.Success)
                    return BadRequest(response);


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau."
                });
            }
        }


        /// <summary>
        /// Endpoint chung: validate OTP cho cả registration & reset password
        /// Query parameter: purpose = "registration" | "resetpassword"
        /// </summary>
        //[HttpPost("validate-otp")]
        //public async Task<IActionResult> ValidateOtp([FromBody] VerifyOtpDto verifyDto, [FromQuery] string purpose = "registration")
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);


        //        _logger.LogInformation("Xác thực OTP: Email={Email}, Purpose={Purpose}", verifyDto.Email, purpose);
        //        var normalizedEmail = verifyDto.Email?.Trim().ToLowerInvariant();

        //        // 1️⃣ Validate OTP
        //        var isValid = await _otpService.ValidateOtpAsync(normalizedEmail, verifyDto.OtpCode, purpose);
        //        if (!isValid)
        //        {
        //            return BadRequest(new AuthResponseDto
        //            {
        //                Success = false,
        //                Message = "Mã OTP không hợp lệ hoặc đã hết hạn."
        //            });
        //        }

        //        // 2️⃣ Get user với normalized email  
        //        var user = await _authService.GetUserByEmailAsync(normalizedEmail);
        //        if (user == null)
        //        {
        //            return BadRequest(new AuthResponseDto
        //            {
        //                Success = false,
        //                Message = "Không tìm thấy tài khoản."
        //            });
        //        }


        //        // 2️⃣ Nếu là registration: update trạng thái user + xóa OTP trong AuthService
        //        if (purpose == "registration")
        //        {
        //            var updateRes = await _authService.VerifyOtpAsync(new VerifyOtpDto
        //            {
        //                Email = verifyDto.Email,
        //                OtpCode = verifyDto.OtpCode
        //            });


        //            if (updateRes?.Success == true)
        //            {
        //                _logger.LogInformation("Đăng ký thành công sau OTP: Email={Email}", verifyDto.Email);
        //                return Ok(new AuthResponseDto
        //                {
        //                    Success = true,
        //                    Message = "OTP hợp lệ. Email đã được xác thực.",
        //                    Token = updateRes.Token,
        //                    UserId = user.Id,
        //                    Email = user.Email,
        //                    RequiresVerification = false
        //                });
        //            }


        //            return BadRequest(updateRes);
        //        }


        //        // 3️⃣ Nếu là reset password: chỉ confirm OTP hợp lệ + xóa OTP luôn
        //        _logger.LogInformation("OTP valid cho reset password: Email={Email}", verifyDto.Email);


        //        try
        //        {
        //            await _otpService.DeleteOtpAsync(verifyDto.Email);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogWarning(ex, "Không xóa được OTP sau khi verify reset password: Email={Email}", verifyDto.Email);
        //        }


        //        return Ok(new AuthResponseDto
        //        {
        //            Success = true,
        //            Message = "OTP hợp lệ. Bạn có thể tiếp tục đặt lại mật khẩu.",
        //            Token = verifyDto.OtpCode,
        //            UserId = user.Id,
        //            Email = user.Email,
        //            RequiresVerification = false
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi xác thực OTP: {Message}", ex.Message);
        //        return StatusCode(500, new AuthResponseDto
        //        {
        //            Success = false,
        //            Message = "Đã xảy ra lỗi khi xác thực OTP."
        //        });
        //    }
        //}
        //[HttpPost("validate-otp/{purpose}")]
        //public async Task<IActionResult> ValidateOtp([FromBody] VerifyOtpDto verifyDto, string purpose)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);
        //        _logger.LogInformation("Xác thực OTP: Email={Email}, Purpose={Purpose}", verifyDto.Email, purpose);
        //        // 1️⃣ Validate OTP
        //        var isValid = await _otpService.ValidateOtpAsync(verifyDto.Email, verifyDto.OtpCode, purpose);
        //        if (!isValid)
        //        {
        //            return BadRequest(new AuthResponseDto
        //            {
        //                Success = false,
        //                Message = "Mã OTP không hợp lệ hoặc đã hết hạn."
        //            });
        //        }
        //        var user = await _authService.GetUserByEmailAsync(verifyDto.Email);
        //        if (user == null)
        //        {
        //            return BadRequest(new AuthResponseDto
        //            {
        //                Success = false,
        //                Message = "Không tìm thấy tài khoản."
        //            });
        //        }
        //        // 2️⃣ Nếu là registration: update trạng thái user + xóa OTP trong AuthService
        //        if (purpose == "registration")
        //        {
        //            var updateRes = await _authService.VerifyOtpAsync(new VerifyOtpDto
        //            {
        //                Email = verifyDto.Email,
        //                OtpCode = verifyDto.OtpCode
        //            });
        //            if (updateRes?.Success == true)
        //            {
        //                _logger.LogInformation("Đăng ký thành công sau OTP: Email={Email}", verifyDto.Email);
        //                return Ok(new AuthResponseDto
        //                {
        //                    Success = true,
        //                    Message = "OTP hợp lệ. Email đã được xác thực.",
        //                    Token = updateRes.Token,
        //                    UserId = user.Id,
        //                    Email = user.Email,
        //                    RequiresVerification = false
        //                });
        //            }
        //            return BadRequest(updateRes);
        //        }
        //        // 3️⃣ Nếu là reset password: chỉ confirm OTP hợp lệ + xóa OTP luôn
        //        _logger.LogInformation("OTP valid cho reset password: Email={Email}", verifyDto.Email);
        //        try
        //        {
        //            await _otpService.DeleteOtpAsync(verifyDto.Email);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogWarning(ex, "Không xóa được OTP sau khi verify reset password: Email={Email}", verifyDto.Email);
        //        }
        //        return Ok(new AuthResponseDto
        //        {
        //            Success = true,
        //            Message = "OTP hợp lệ. Bạn có thể tiếp tục đặt lại mật khẩu.",
        //            Token = verifyDto.OtpCode,
        //            UserId = user.Id,
        //            Email = user.Email,
        //            RequiresVerification = false
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi xác thực OTP: {Message}", ex.Message);
        //        return StatusCode(500, new AuthResponseDto
        //        {
        //            Success = false,
        //            Message = "Đã xảy ra lỗi khi xác thực OTP."
        //        });
        //    }
        //}
        [HttpPost("validate-otp/{purpose}")]
        public async Task<IActionResult> ValidateOtp([FromBody] VerifyOtpDto verifyDto, string purpose)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // ✅ TC4, TC5: Validate email
                if (string.IsNullOrWhiteSpace(verifyDto.Email))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email is required."
                    });
                }

                // ✅ TC9, TC10: Validate OTP
                if (string.IsNullOrWhiteSpace(verifyDto.OtpCode))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "OTP code is required."
                    });
                }

                _logger.LogInformation("Xác thực OTP: Email={Email}, Purpose={Purpose}", verifyDto.Email, purpose);

                // ✅ TC6, TC8, TC11: Validate OTP (sai/hết hạn/không tồn tại)
                var isValid = await _otpService.ValidateOtpAsync(verifyDto.Email, verifyDto.OtpCode, purpose);
                if (!isValid)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "OTP code is invalid or expired."
                    });
                }

                // ✅ TC3: Check user exists
                var user = await _authService.GetUserByEmailAsync(verifyDto.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                // Registration purpose: update user status
                if (purpose == "registration")
                {
                    var updateRes = await _authService.VerifyOtpAsync(new VerifyOtpDto
                    {
                        Email = verifyDto.Email,
                        OtpCode = verifyDto.OtpCode
                    });

                    if (updateRes?.Success == true)
                    {
                        _logger.LogInformation("Đăng ký thành công sau OTP: Email={Email}", verifyDto.Email);
                        return Ok(new AuthResponseDto
                        {
                            Success = true,
                            Message = "Email verified successfully.",
                            Token = updateRes.Token,
                            UserId = user.Id,
                            Email = user.Email,
                            RequiresVerification = false
                        });
                    }

                    return BadRequest(updateRes);
                }

                // Reset password purpose: just confirm OTP is valid
                _logger.LogInformation("OTP valid for reset password: Email={Email}", verifyDto.Email);

                try
                {
                    await _otpService.DeleteOtpAsync(verifyDto.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete OTP: Email={Email}", verifyDto.Email);
                }

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "OTP verified.  You can now reset your password.",
                    Token = verifyDto.OtpCode,
                    UserId = user.Id,
                    Email = user.Email,
                    RequiresVerification = false
                });
            }
            catch (Exception ex)
            {
                // ✅ TC14: Server error
                _logger.LogError(ex, "Error validating OTP: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred while validating OTP."
                });
            }
        }
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] EmailRequestDto resendOtpDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var user = await _authService.GetUserByEmailAsync(resendOtpDto.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy tài khoản với email này."
                    });
                }


                if (await _otpService.IsOtpLimitExceededAsync(resendOtpDto.Email))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Bạn đã yêu cầu gửi lại OTP quá nhiều lần. Vui lòng thử lại sau."
                    });
                }


                var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "registration");
                await _emailService.SendVerificationEmailAsync(user.Email, otp);


                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Mã xác thực đã được gửi đến email của bạn.",
                    UserId = user.Id,
                    RequiresVerification = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi lại OTP: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi gửi lại OTP. Vui lòng thử lại sau."
                });
            }
        }


        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();


                _logger.LogInformation("Đăng nhập Google: IP={IP}, UA={UA}", ipAddress, userAgent);


                var response = await _authService.GoogleLoginAsync(googleLoginDto.IdToken);


                if (!response.Success)
                    return BadRequest(response);


                // ✅ Initialize default gestures for Google user
                if (!string.IsNullOrEmpty(response.UserId))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _gestureInitService.CreateUserDataFolderAsync(response.UserId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to initialize gestures for Google user: {UserId}", response.UserId);
                        }
                    });
                }


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập bằng Google: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi đăng nhập bằng Google. Vui lòng thử lại sau."
                });
            }
        }


        // ✅ Thống kê gesture (đã có sẵn)
        [HttpGet("gestures/stats")]
        [Authorize]
        public async Task<IActionResult> GetGestureStats()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }


                var userId = userIdClaim.Value;
                var stats = await _gestureInitService.GetUserGestureStatsAsync(userId);


                return Ok(new
                {
                    success = true,
                    data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gesture stats");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailRequestDto emailRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var response = await _authService.ForgotPasswordAsync(emailRequestDto.Email);


                if (!response.Success)
                    return BadRequest(response);


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi yêu cầu đặt lại mật khẩu: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi yêu cầu đặt lại mật khẩu. Vui lòng thử lại sau."
                });
            }
        }


        [HttpPost("resend-reset-otp")]
        public async Task<IActionResult> ResendResetOtp([FromBody] EmailRequestDto emailRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var user = await _authService.GetUserByEmailAsync(emailRequestDto.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy tài khoản với email này."
                    });
                }


                if (await _otpService.IsOtpLimitExceededAsync(emailRequestDto.Email))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Bạn đã yêu cầu gửi lại mã quá nhiều lần. Vui lòng thử lại sau."
                    });
                }


                var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "resetpassword");
                await _emailService.SendPasswordResetEmailAsync(user.Email, otp);


                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Mã OTP đã được gửi lại đến email của bạn.",
                    UserId = user.Id,
                    RequiresVerification = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi lại OTP reset: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi gửi lại mã OTP. Vui lòng thử lại sau."
                });
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var response = await _authService.ResetPasswordAsync(resetDto);


                if (!response.Success)
                    return BadRequest(response);


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi đặt lại mật khẩu. Vui lòng thử lại sau."
                });
            }
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy thông tin người dùng trong token."
                    });
                }


                var userId = userIdClaim.Value;


                _logger.LogInformation("Người dùng đăng xuất: UserId={UserId}", userId);


                var response = await _authService.LogoutAsync(userId);


                if (!response.Success)
                    return BadRequest(response);


                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng xuất: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi đăng xuất. Vui lòng thử lại sau."
                });
            }
        }
    }
}





