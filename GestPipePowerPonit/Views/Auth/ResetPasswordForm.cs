using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using Guna.UI2.WinForms;

namespace GestPipePowerPonit.Views.Auth
{
    public partial class ResetPasswordForm : Form
    {
        private readonly AuthService _authService;
        private readonly string _email;
        private readonly string _resetToken;
        private readonly LoginForm _loginForm;

        public ResetPasswordForm(LoginForm loginForm = null, string email = "", string resetToken = "")
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _email = email;
            _resetToken = resetToken;
            _loginForm = loginForm;
            _authService = new AuthService();
        }

        private void ResetPasswordForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Properties.Resources.background;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.picLogo.Image = Properties.Resources.Logo;
            }
            catch { this.BackColor = Color.Black; }

            // ✅ Load text từ .resx
            lblTitle.Text = AppSettings.GetText("ResetPasswordForm_Title");
            txtNewPassword.PlaceholderText = AppSettings.GetText("ResetPasswordForm_NewPassword");
            txtConfirmPassword.PlaceholderText = AppSettings.GetText("ResetPasswordForm_ConfirmPassword");
            btnResetPassword.Text = AppSettings.GetText("ResetPasswordForm_Reset");
            lnkLogin.Text = AppSettings.GetText("ResetPasswordForm_Login");

            // ✅ Bỏ gạch chân link
            lnkLogin.LinkBehavior = LinkBehavior.NeverUnderline;

            CenterControls();
        }

        private void ResetPasswordForm_Resize(object sender, EventArgs e)
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
            lblNewPasswordError.Visible = false;
            lblConfirmPasswordError.Visible = false;
            txtNewPassword.BorderColor = Color.FromArgb(220, 220, 220);
            txtConfirmPassword.BorderColor = Color.FromArgb(220, 220, 220);
        }

        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();

            var resetDto = new ResetPasswordDto
            {
                Email = _email,
                NewPassword = txtNewPassword.Text,
                ConfirmPassword = txtConfirmPassword.Text
            };

            // ✅ Validate theo Data Annotations
            var context = new ValidationContext(resetDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(resetDto, context, validationResults, true);

            System.Diagnostics.Debug.WriteLine($"[Validation] Valid={isValid}, Errors={validationResults.Count}");

            var newPasswordErrors = new List<string>();
            var confirmPasswordErrors = new List<string>();

            foreach (var result in validationResults)
            {
                System.Diagnostics.Debug.WriteLine($"  └─ Member: {string.Join(", ", result.MemberNames)}");

                if (result.MemberNames.Contains(nameof(ResetPasswordDto.NewPassword)))
                {
                    if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
                    {
                        newPasswordErrors.Add(AppSettings.GetText("Validation_EmptyPassword"));
                    }
                    else
                    {
                        newPasswordErrors.Add(AppSettings.GetText("Validation_PasswordTooShort"));
                    }
                }

                if (result.MemberNames.Contains(nameof(ResetPasswordDto.ConfirmPassword)))
                {
                    if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
                    {
                        confirmPasswordErrors.Add(AppSettings.GetText("Validation_EmptyConfirmPassword"));
                    }
                    else
                    {
                        confirmPasswordErrors.Add(AppSettings.GetText("Validation_PasswordMismatch"));
                    }
                }
            }

            // ✅ Kiểm tra password không khớp
            if (!string.IsNullOrWhiteSpace(txtNewPassword.Text) &&
                !string.IsNullOrWhiteSpace(txtConfirmPassword.Text) &&
                txtNewPassword.Text != txtConfirmPassword.Text)
            {
                confirmPasswordErrors.Add(AppSettings.GetText("Validation_PasswordMismatch"));
            }

            // ✅ Hiển thị lỗi
            if (newPasswordErrors.Any())
            {
                lblNewPasswordError.Text = string.Join("\n", newPasswordErrors);
                lblNewPasswordError.Visible = true;
                txtNewPassword.BorderColor = Color.OrangeRed;
                errors.AddRange(newPasswordErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ NewPassword Error: {lblNewPasswordError.Text}");
            }

            if (confirmPasswordErrors.Any())
            {
                lblConfirmPasswordError.Text = string.Join("\n", confirmPasswordErrors);
                lblConfirmPasswordError.Visible = true;
                txtConfirmPassword.BorderColor = Color.OrangeRed;
                errors.AddRange(confirmPasswordErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ ConfirmPassword Error: {lblConfirmPasswordError.Text}");
            }

            return errors.Count == 0;
        }

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            // ✅ Validate trước khi gửi API
            if (!ValidateInputs(out var validationErrors))
            {
                return;
            }

            btnResetPassword.Enabled = false;
            btnResetPassword.Text = AppSettings.GetText("ResetPasswordForm_Resetting") ?? "Resetting...";

            try
            {
                // ✅ SỬA: Chỉ gửi Email, NewPassword, ConfirmPassword
                var res = await _authService.ResetPasswordAsync(
                    _email,
                    txtNewPassword.Text,        // ✅ NewPassword
                    txtConfirmPassword.Text     // ✅ ConfirmPassword
                );

                if (res?.Success == true)
                {
                    // ✅ NEW: Custom MessageBox Success
                    CustomMessageBox.ShowSuccess(
                        res.Message ?? AppSettings.GetText("Message_ResetPasswordSuccess"),
                        AppSettings.GetText("Title_Success") ?? "Success"
                    );

                    // ✅ Show LoginForm trước khi close
                    if (_loginForm != null && !_loginForm.IsDisposed)
                    {
                        _loginForm.Show();
                    }

                    this.Close();
                }
                else
                {
                    // ✅ Parse lỗi từ API response
                    string errorMsg = res?.Message ?? AppSettings.GetText("Message_ResetPasswordFailed");

                    // ✅ Kiểm tra lỗi "password không khớp" từ backend
                    if (errorMsg.Contains("không khớp") || errorMsg.Contains("not match"))
                    {
                        lblConfirmPasswordError.Text = AppSettings.GetText("Validation_PasswordMismatch");
                        lblConfirmPasswordError.Visible = true;
                        txtConfirmPassword.BorderColor = Color.OrangeRed;
                    }

                    // ✅ NEW: Custom MessageBox Error
                    CustomMessageBox.ShowError(
                        errorMsg,
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );
                }
            }
            catch (Exception ex)
            {
                // ✅ NEW: Custom MessageBox Error
                CustomMessageBox.ShowError(
                    $"Error: {ex.Message}",
                    AppSettings.GetText("Title_ConnectionError") ?? "Connection Error"
                );
            }
            finally
            {
                btnResetPassword.Enabled = true;
                btnResetPassword.Text = AppSettings.GetText("ResetPasswordForm_Reset");
            }
        }

        private void txtNewPassword_IconRightClick(object sender, EventArgs e)
        {
            txtNewPassword.PasswordChar = txtNewPassword.PasswordChar == '\0' ? '●' : '\0';
        }

        private void txtConfirmPassword_IconRightClick(object sender, EventArgs e)
        {
            txtConfirmPassword.PasswordChar = txtConfirmPassword.PasswordChar == '\0' ? '●' : '\0';
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // ✅ Show LoginForm trước khi close
            if (_loginForm != null && !_loginForm.IsDisposed)
            {
                _loginForm.Show();
            }

            this.Close();
        }

        private void txtNewPassword_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtConfirmPassword_TextChanged(object sender, EventArgs e) => ClearErrors();
    }
}