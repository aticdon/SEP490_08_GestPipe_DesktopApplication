using GestPipe.Backend.Models.DTOs;
using GestPipe.Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GestPipe.Backend.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;

namespace GestPipe.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IGestureInitializationService _gestureInitService; // ✅ ADD
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IOtpService otpService,
            IEmailService emailService,
            IGestureInitializationService gestureInitService, // ✅ ADD
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _otpService = otpService;
            _emailService = emailService;
            _gestureInitService = gestureInitService; // ✅ ADD
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Đăng ký tài khoản mới: Email={Email}, FullName={FullName}",
                    registerDto.Email, registerDto.FullName);

                var response = await _authService.RegisterAsync(registerDto);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                 //✅ INITIALIZE DEFAULT GESTURES(async, don't wait)
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
                {
                    return BadRequest(ModelState);
                }

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await _authService.LoginAsync(loginDto, ipAddress, userAgent);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

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
        /// 👇 ENDPOINT CHUNG - Xác thực OTP cho cả registration và reset password
        /// Query parameter: purpose = "registration" hoặc "resetpassword"
        /// </summary>
        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp([FromBody] VerifyOtpDto verifyDto, [FromQuery] string purpose = "registration")
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Xác thực OTP: Email={Email}, Purpose={Purpose}", verifyDto.Email, purpose);

                // 👇 KIỂM TRA OTP CÓ HỢP LỆ KHÔNG
                var isValid = await _otpService.ValidateOtpAsync(verifyDto.Email, verifyDto.OtpCode, purpose);
                if (!isValid)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Mã OTP không hợp lệ hoặc đã hết hạn."
                    });
                }

                // 👇 MARK OTP AS VERIFIED
                var marked = await _otpService.MarkOtpAsVerifiedAsync(verifyDto.Email, verifyDto.OtpCode);
                if (!marked)
                {
                    _logger.LogWarning("Failed to mark OTP as verified for email: {Email}", verifyDto.Email);
                }

                var user = await _authService.GetUserByEmailAsync(verifyDto.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy tài khoản."
                    });
                }

                // 👇 NẾU LÀ REGISTRATION - CẬP NHẬT TRẠNG THÁI NGƯỜI DÙNG
                if (purpose == "registration")
                {
                    var updateRes = await _authService.VerifyOtpAsync(new VerifyOtpDto
                    {
                        Email = verifyDto.Email,
                        OtpCode = verifyDto.OtpCode
                    });

                    if (updateRes?.Success == true)
                    {
                        _logger.LogInformation("Đăng ký thành công: Email={Email}", verifyDto.Email);
                        return Ok(new AuthResponseDto
                        {
                            Success = true,
                            Message = "OTP hợp lệ. Email đã được xác thực.",
                            Token = updateRes.Token,
                            UserId = user.Id,
                            Email = user.Email,
                            RequiresVerification = false
                        });
                    }
                }

                // 👇 NẾU LÀ RESET PASSWORD - CHỈ CONFIRM OTP HỢP LỆ
                _logger.LogInformation("OTP được xác thực cho reset password: Email={Email}", verifyDto.Email);
                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "OTP hợp lệ. Bạn có thể tiếp tục đặt lại mật khẩu.",
                    Token = verifyDto.OtpCode,
                    UserId = user.Id,
                    Email = user.Email,
                    RequiresVerification = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực OTP: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xác thực OTP."
                });
            }
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] EmailRequestDto resendOtpDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _authService.GetUserByEmailAsync(resendOtpDto.Email);
                if (user == null)
                {
                    return BadRequest(new AuthResponseDto { Success = false, Message = "Không tìm thấy tài khoản với email này." });
                }

                if (await _otpService.IsOtpLimitExceededAsync(resendOtpDto.Email))
                {
                    return BadRequest(new AuthResponseDto { Success = false, Message = "Bạn đã yêu cầu gửi lại OTP quá nhiều lần. Vui lòng thử lại sau." });
                }

                var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "registration");
                await _emailService.SendVerificationEmailAsync(user.Email, otp);

                return Ok(new AuthResponseDto { Success = true, Message = "Mã xác thực đã được gửi đến email của bạn.", UserId = user.Id, RequiresVerification = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi lại OTP: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto { Success = false, Message = "Đã xảy ra lỗi khi gửi lại OTP. Vui lòng thử lại sau." });
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto googleLoginDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var response = await _authService.GoogleLoginAsync(googleLoginDto.IdToken, ipAddress, userAgent);

                if (!response.Success) return BadRequest(response);

                // ✅ INITIALIZE DEFAULT GESTURES FOR NEW GOOGLE USER
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
                return StatusCode(500, new AuthResponseDto { Success = false, Message = "Đã xảy ra lỗi khi đăng nhập bằng Google. Vui lòng thử lại sau." });
            }
        }
        // ✅ NEW ENDPOINT: Get gesture statistics
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
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var response = await _authService.ForgotPasswordAsync(emailRequestDto.Email);

                if (!response.Success) return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi yêu cầu đặt lại mật khẩu: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto { Success = false, Message = "Đã xảy ra lỗi khi yêu cầu đặt lại mật khẩu. Vui lòng thử lại sau." });
            }
        }

        [HttpPost("resend-reset-otp")]
        public async Task<IActionResult> ResendResetOtp([FromBody] EmailRequestDto emailRequestDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _authService.GetUserByEmailAsync(emailRequestDto.Email);
                if (user == null) return BadRequest(new AuthResponseDto { Success = false, Message = "Không tìm thấy tài khoản với email này." });

                if (await _otpService.IsOtpLimitExceededAsync(emailRequestDto.Email))
                {
                    return BadRequest(new AuthResponseDto { Success = false, Message = "Bạn đã yêu cầu gửi lại mã quá nhiều lần. Vui lòng thử lại sau." });
                }

                var otp = await _otpService.GenerateOtpAsync(user.Id, user.Email, "resetpassword");
                await _emailService.SendPasswordResetEmailAsync(user.Email, otp);

                return Ok(new AuthResponseDto { Success = true, Message = "Mã OTP đã được gửi lại đến email của bạn.", UserId = user.Id, RequiresVerification = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi lại OTP reset: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto { Success = false, Message = "Đã xảy ra lỗi khi gửi lại mã OTP. Vui lòng thử lại sau." });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var response = await _authService.ResetPasswordAsync(resetDto);

                if (!response.Success) return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt lại mật khẩu: {Message}", ex.Message);
                return StatusCode(500, new AuthResponseDto { Success = false, Message = "Đã xảy ra lỗi khi đặt lại mật khẩu. Vui lòng thử lại sau." });
            }
        }
        [HttpPost("logout")]
        [Authorize] // ✅ REQUIRE JWT TOKEN
        public async Task<IActionResult> Logout()
        {
            try
            {
                // 👇 LẤY USER ID TỪ JWT CLAIMS
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
                {
                    return BadRequest(response);
                }

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