using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;  // ✅ THÊM để sử dụng CustomMessageBox
using Guna.UI2.WinForms;

namespace GestPipePowerPonit.Views.Auth
{
    public partial class ForgotPasswordForm : Form
    {
        private AuthService _authService;
        private LoginForm _loginForm;

        public ForgotPasswordForm(LoginForm loginForm = null)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _authService = new AuthService();
            _loginForm = loginForm;
        }

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Properties.Resources.background;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.picLogo.Image = Properties.Resources.Logo;
            }
            catch { this.BackColor = Color.Black; }

            // ✅ Load text từ .resx
            lblTitle.Text = AppSettings.GetText("ForgotPasswordForm_Title");
            txtEmail.PlaceholderText = AppSettings.GetText("ForgotPasswordForm_EmailPlaceholder");
            btnSendOtp.Text = AppSettings.GetText("ForgotPasswordForm_SendOtp");
            lblLogin.Text = AppSettings.GetText("ForgotPasswordForm_Login");

            // ✅ Bỏ gạch chân link
            lblLogin.LinkBehavior = LinkBehavior.NeverUnderline;

            CenterControls();
        }

        private void ForgotPasswordForm_Resize(object sender, EventArgs e)
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
                pnlCard.Top - picLogo.Height);  // ✅ Bỏ -20
        }

        // ===== Validation =====
        private void ClearErrors()
        {
            lblEmailError.Visible = false;
            txtEmail.BorderColor = Color.FromArgb(220, 220, 220);
        }

        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();

            // Create DTO from input
            var emailDto = new EmailRequestDto
            {
                Email = txtEmail.Text.Trim()
            };

            // ✅ Validate according to Data Annotations
            var context = new ValidationContext(emailDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(emailDto, context, validationResults, true);

            System.Diagnostics.Debug.WriteLine($"[Validation] Valid={isValid}, Errors={validationResults.Count}");

            // ✅ Map errors from .resx file instead of from DTO
            var emailErrors = new List<string>();

            foreach (var result in validationResults)
            {
                System.Diagnostics.Debug.WriteLine($"  └─ Member: {string.Join(", ", result.MemberNames)}");

                // Check for Email errors
                if (result.MemberNames.Contains(nameof(EmailRequestDto.Email)))
                {
                    if (string.IsNullOrWhiteSpace(txtEmail.Text))
                    {
                        emailErrors.Add(AppSettings.GetText("Validation_EmptyEmail"));
                    }
                    else
                    {
                        emailErrors.Add(AppSettings.GetText("Validation_InvalidEmail"));
                    }
                }
            }

            // ✅ Display email error if any
            if (emailErrors.Any())
            {
                lblEmailError.Text = string.Join("\n", emailErrors);
                lblEmailError.Visible = true;
                txtEmail.BorderColor = Color.OrangeRed;
                errors.AddRange(emailErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ Email Error: {lblEmailError.Text}");
            }

            return errors.Count == 0;
        }

        private async void btnSendOtp_Click(object sender, EventArgs e)
        {
            // ✅ Validate before sending API request
            if (!ValidateInputs(out var validationErrors))
            {
                // Error is displayed in form only
                return;
            }

            btnSendOtp.Enabled = false;
            btnSendOtp.Text = AppSettings.GetText("ForgotPasswordForm_Sending") ?? "Sending OTP...";

            try
            {
                var response = await _authService.ForgotPasswordAsync(txtEmail.Text.Trim());

                if (response?.Success == true)
                {
                    // ✅ Sử dụng CustomMessageBox thay vì MessageBox
                    CustomMessageBox.ShowSuccess(
                        response.Message ?? AppSettings.GetText("Message_OtpSendSuccess"),
                        AppSettings.GetText("Title_Success") ?? "Success"
                    );

                    // ✅ Close ForgotPasswordForm trước
                    this.Close();

                    // ✅ PASS LOGINFORM TO VERIFYOTPFORM
                    var verify = new VerifyOtpForm(_loginForm, txtEmail.Text.Trim(), isRegistration: false);
                    verify.ShowDialog();
                }
                else
                {
                    // ✅ Sử dụng CustomMessageBox cho error
                    CustomMessageBox.ShowError(
                        response?.Message ?? AppSettings.GetText("Message_OtpSendFailed") ?? "Failed to send OTP!",
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );
                }
            }
            catch (Exception ex)
            {
                // ✅ Sử dụng CustomMessageBox cho exception
                CustomMessageBox.ShowError(
                    $"{AppSettings.GetText("Title_Error")}: {ex.Message}",
                    AppSettings.GetText("Title_ConnectionError") ?? "Connection Error"
                );
            }
            finally
            {
                btnSendOtp.Enabled = true;
                btnSendOtp.Text = AppSettings.GetText("ForgotPasswordForm_SendOtp") ?? "Send OTP";
            }
        }

        private void lblLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // ✅ SHOW LOGIN FORM
            if (_loginForm != null && !_loginForm.IsDisposed)
            {
                _loginForm.Show();
            }

            this.Close();
        }

        private void txtEmail_TextChanged(object sender, EventArgs e) => ClearErrors();
    }
}