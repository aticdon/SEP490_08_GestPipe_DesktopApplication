using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using Guna.UI2.WinForms;

namespace GestPipePowerPonit.Views.Auth
{
    public partial class VerifyOtpForm : Form
    {
        // OTP configuration
        private readonly int _otpLength = 6;
        private readonly int _otpExpiryMinutes = 5;
        private readonly int _maxOtpPerHour = 3;

        private readonly AuthService _authService;
        private readonly string _email;
        private readonly bool _isRegistration;
        private readonly LoginForm _loginForm;

        // Countdown timer / expiry
        private Timer _otpTimer;
        private DateTime _otpExpiresAt;
        private bool _otpVerifiedSuccessfully = false;
        public VerifyOtpForm(LoginForm loginForm = null, string email = "", bool isRegistration = false)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _email = email;
            _isRegistration = isRegistration;
            _loginForm = loginForm;
            _authService = new AuthService();

            // Timer used for the OTP countdown (UI timer)
            _otpTimer = new Timer { Interval = 1000 }; // 1 second
            _otpTimer.Tick += OtpTimer_Tick;
            this.FormClosing += VerifyOtpForm_FormClosing;
        }

        private void VerifyOtpForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Properties.Resources.background;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.picLogo.Image = Properties.Resources.Logo;
            }
            catch { this.BackColor = Color.Black; }

            // ✅ Load text từ .resx
            lblTitle.Text = AppSettings.GetText("VerifyOtpForm_Title");
            txtOtp.PlaceholderText = AppSettings.GetText("VerifyOtpForm_OtpPlaceholder");
            btnVerify.Text = AppSettings.GetText("VerifyOtpForm_Verify");
            lnkResend.Text = AppSettings.GetText("VerifyOtpForm_Resend");

            // ✅ Bỏ gạch chân link
            lnkResend.LinkBehavior = LinkBehavior.NeverUnderline;

            // Start countdown as soon as form loads (assumes OTP was just sent)
            StartOtpCountdown();

            CenterControls();
        }
        private async void VerifyOtpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Chỉ xử lý case ĐANG REGISTER & CHƯA VERIFY
            if (_isRegistration && !_otpVerifiedSuccessfully)
            {
                try
                {
                    // Gọi API xóa account chưa verify
                    // (tên hàm và endpoint bạn tự chọn cho phù hợp backend)
                    await _authService.CancelPendingRegistrationAsync(_email);

                    // Không show message ở đây cũng được, vì user đang đóng form
                    Console.WriteLine($"[VerifyOtpForm] Canceled pending registration for email: {_email}");
                }
                catch (Exception ex)
                {
                    // Đừng chặn đóng form, chỉ log
                    Console.WriteLine($"[VerifyOtpForm] Error when cancel registration: {ex.Message}");
                }
            }
        }


        private void VerifyOtpForm_Resize(object sender, EventArgs e)
        {
            CenterControls();
        }

        private void CenterControls()
        {
            pnlCard.Location = new Point(
                (this.ClientSize.Width - pnlCard.Width) / 2,
                (this.ClientSize.Height - pnlCard.Height) / 2);

            picLogo.Location = new Point(
                (this.ClientSize.Width - picLogo.Width) / 2,
                pnlCard.Top - picLogo.Height);
        }

        // ===== Countdown logic =====
        private void StartOtpCountdown()
        {
            // Set expiry time relative to now (UTC to be safer in server scenarios)
            _otpExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes);
            UpdateTimerLabel();
            btnVerify.Enabled = true;
            _otpTimer.Start();
        }

        private void StopOtpCountdown()
        {
            _otpTimer.Stop();
        }

        private void ResetOtpCountdown()
        {
            StopOtpCountdown();
            StartOtpCountdown();
        }

        private void OtpTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan remaining = _otpExpiresAt - DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
            {
                // expired
                StopOtpCountdown();
                lblTimer.Text = AppSettings.GetText("VerifyOtpForm_Expired") ?? "Expired";
                lblTimer.ForeColor = Color.IndianRed;
                btnVerify.Enabled = false;

                // show small notice (optional)
                // CustomMessageBox.ShowWarning(AppSettings.GetText("Message_OtpExpired") ?? "OTP has expired. Please resend.");

                return;
            }

            // update label with mm:ss
            lblTimer.Text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            lblTimer.ForeColor = Color.White;
        }

        private void UpdateTimerLabel()
        {
            TimeSpan remaining = _otpExpiresAt - DateTime.UtcNow;
            if (remaining <= TimeSpan.Zero)
            {
                lblTimer.Text = AppSettings.GetText("VerifyOtpForm_Expired") ?? "Expired";
                lblTimer.ForeColor = Color.IndianRed;
            }
            else
            {
                lblTimer.Text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                lblTimer.ForeColor = Color.White;
            }
        }

        // ===== Validation =====
        private void ClearErrors()
        {
            lblOtpError.Visible = false;
            txtOtp.BorderColor = Color.FromArgb(220, 220, 220);
        }

        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();

            var otpDto = new VerifyOtpDto
            {
                Email = _email,
                OtpCode = txtOtp.Text.Trim()
            };

            var context = new ValidationContext(otpDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(otpDto, context, validationResults, true);

            System.Diagnostics.Debug.WriteLine($"[Validation] Valid={isValid}, Errors={validationResults.Count}");

            var otpErrors = new List<string>();

            foreach (var result in validationResults)
            {
                System.Diagnostics.Debug.WriteLine($"  └─ Member: {string.Join(", ", result.MemberNames)}");

                if (result.MemberNames.Contains(nameof(VerifyOtpDto.OtpCode)))
                {
                    if (string.IsNullOrWhiteSpace(txtOtp.Text))
                    {
                        otpErrors.Add(AppSettings.GetText("Validation_EmptyOtp"));
                    }
                    else if (txtOtp.Text.Length != _otpLength)
                    {
                        otpErrors.Add(AppSettings.GetText("Validation_InvalidOtpLength"));
                    }
                    else
                    {
                        otpErrors.Add(AppSettings.GetText("Validation_InvalidOtp"));
                    }
                }
            }

            // ✅ Hiển thị lỗi OTP nếu có
            if (otpErrors.Any())
            {
                lblOtpError.Text = string.Join("\n", otpErrors);
                lblOtpError.Visible = true;
                txtOtp.BorderColor = Color.OrangeRed;
                errors.AddRange(otpErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ OTP Error: {lblOtpError.Text}");
            }

            return errors.Count == 0;
        }

        private async void btnVerify_Click(object sender, EventArgs e)
        {
            // ✅ Validate trước khi gửi API
            if (!ValidateInputs(out var validationErrors))
            {
                return;
            }

            // if already expired, prevent sending
            if (DateTime.UtcNow >= _otpExpiresAt)
            {
                lblOtpError.Text = AppSettings.GetText("Validation_OtpExpired") ?? "OTP has expired. Please resend.";
                lblOtpError.Visible = true;
                txtOtp.BorderColor = Color.OrangeRed;
                return;
            }

            btnVerify.Enabled = false;
            btnVerify.Text = AppSettings.GetText("VerifyOtpForm_Verifying") ?? "Verifying...";

            try
            {
                string purpose = _isRegistration ? "registration" : "resetpassword";
                var res = await _authService.ValidateOtpAsync(_email, txtOtp.Text.Trim(), purpose);

                if (res?.Success == true)
                {
                    // stop countdown on success
                    StopOtpCountdown();

                    _otpVerifiedSuccessfully = true;

                    if (_isRegistration)
                    {
                        // ✅ REGISTRATION FLOW
                        CustomMessageBox.ShowSuccess(
                            AppSettings.GetText("Message_RegistrationSuccess") ?? "Registration completed successfully!",
                            AppSettings.GetText("Title_Success") ?? "Success"
                        );

                        this.Close();

                        if (_loginForm != null && !_loginForm.IsDisposed)
                        {
                            _loginForm.Show();
                        }
                    }
                    else
                    {
                        // ✅ RESET PASSWORD FLOW
                        var resetToken = res.Token;
                        if (string.IsNullOrEmpty(resetToken))
                        {
                            CustomMessageBox.ShowError(
                                AppSettings.GetText("Message_NoResetToken") ?? "No reset token received from server.",
                                AppSettings.GetText("Title_Error") ?? "Error"
                            );
                            return;
                        }

                        // ✅ Show success message
                        CustomMessageBox.ShowSuccess(
                            AppSettings.GetText("Message_OtpVerified") ?? "OTP verified successfully!",
                            AppSettings.GetText("Title_Success") ?? "Success"
                        );

                        this.Close();

                        var resetForm = new ResetPasswordForm(_loginForm, _email, resetToken);
                        resetForm.ShowDialog();
                    }
                }
                else
                {
                    // ✅ Hiển thị lỗi từ backend
                    string errorMsg = res?.Message ?? AppSettings.GetText("Message_OtpVerifyFailed");

                    // ✅ Parse lỗi OTP không hợp lệ
                    if (errorMsg.Contains("OTP") || errorMsg.Contains("invalid") || errorMsg.Contains("không hợp lệ"))
                    {
                        lblOtpError.Text = AppSettings.GetText("Validation_InvalidOtp");
                        lblOtpError.Visible = true;
                        txtOtp.BorderColor = Color.OrangeRed;
                    }

                    CustomMessageBox.ShowError(
                        errorMsg,
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError(
                    $"Error: {ex.Message}",
                    AppSettings.GetText("Title_ConnectionError") ?? "Connection Error"
                );
            }
            finally
            {
                btnVerify.Enabled = true;
                btnVerify.Text = AppSettings.GetText("VerifyOtpForm_Verify");
            }
        }

        private async void lnkResend_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkResend.Enabled = false;
            lnkResend.Text = AppSettings.GetText("VerifyOtpForm_Sending") ?? "Sending...";

            try
            {
                AuthResponseDto res = null;

                if (_isRegistration)
                {
                    res = await _authService.ResendOtpAsync(_email);
                }
                else
                {
                    res = await _authService.ResendResetOtpAsync(_email);
                }

                if (res?.Success == true)
                {
                    // restart the countdown when OTP resent
                    ResetOtpCountdown();

                    CustomMessageBox.ShowSuccess(
                        res.Message ?? AppSettings.GetText("Message_OtpResent"),
                        AppSettings.GetText("Title_Success") ?? "Success"
                    );
                }
                else
                {
                    CustomMessageBox.ShowError(
                        res?.Message ?? AppSettings.GetText("Message_OtpResendFailed"),
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError(
                    $"Error: {ex.Message}",
                    AppSettings.GetText("Title_ConnectionError") ?? "Connection Error"
                );
            }
            finally
            {
                lnkResend.Enabled = true;
                lnkResend.Text = AppSettings.GetText("VerifyOtpForm_Resend");
            }
        }

        private void txtOtp_TextChanged(object sender, EventArgs e) => ClearErrors();

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}