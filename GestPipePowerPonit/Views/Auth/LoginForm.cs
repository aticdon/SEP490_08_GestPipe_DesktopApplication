using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GestPipePowerPonit.Views.Auth
{
    public partial class LoginForm : Form
    {
        private AuthService _authService;

        public LoginForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _authService = new AuthService();
            this.Size = new Size(1366, 786);
            this.MinimumSize = new Size(1366, 786);
            this.MaximumSize = new Size(1366, 786);
            this.StartPosition = FormStartPosition.CenterScreen;
            // Login form luôn dùng tiếng Anh (EN)

            AppSettings.CurrentLanguage = "EN";
            AppSettings.SetLanguage("EN");
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Properties.Resources.background;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                this.picLogo.Image = Properties.Resources.Logo;

            }
            catch { this.BackColor = Color.Black; }

            // Tất cả text = tiếng Anh
            lblTitle.Text = AppSettings.GetText("LoginForm_Title");
            btnLogin.Text = AppSettings.GetText("LoginForm_BtnLogin");
            lblForgotPassword.Text = AppSettings.GetText("LoginForm_ForgotPassword");
            lblNoAccount.Text = AppSettings.GetText("LoginForm_NoAccount");
            lblRegister.Text = AppSettings.GetText("LoginForm_Register");
            lblRegister.LinkBehavior = LinkBehavior.NeverUnderline;
            lblForgotPassword.LinkBehavior = LinkBehavior.NeverUnderline;
            btnGoogleLogin.Text = AppSettings.GetText("LoginForm_GoogleBtn");

            CenterControls();
        }

        private void LoginForm_Resize(object sender, EventArgs e) => CenterControls();

        private void CenterControls()
        {
            pnlCard.Location = new Point(
                (this.ClientSize.Width - pnlCard.Width) / 2,
                (this.ClientSize.Height - pnlCard.Height) / 2);

            picLogo.Location = new Point(
                (this.ClientSize.Width - picLogo.Width) / 2,
                pnlCard.Top - picLogo.Height);
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = txtPassword.PasswordChar == '\0' ? '●' : '\0';
        }

        // ===== Validation =====
        private void ClearErrors()
        {
            lblUserError.Visible = false;
            lblPwdError.Visible = false;
            txtUserName.BorderColor = Color.FromArgb(220, 220, 220);
            txtPassword.BorderColor = Color.FromArgb(220, 220, 220);
        }

        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();

            // Tạo DTO từ input
            var loginDto = new LoginDto
            {
                Email = txtUserName.Text.Trim(),
                Password = txtPassword.Text
            };

            // ✅ Validate theo Data Annotations
            var context = new ValidationContext(loginDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(loginDto, context, validationResults, true);

            System.Diagnostics.Debug.WriteLine($"[Validation] Valid={isValid}, Errors={validationResults.Count}");

            // ✅ Map error từ .resx file thay vì từ DTO
            var emailErrors = new List<string>();
            var passwordErrors = new List<string>();

            foreach (var result in validationResults)
            {
                System.Diagnostics.Debug.WriteLine($"  └─ Member: {string.Join(", ", result.MemberNames)}");

                // Kiểm tra lỗi Email
                if (result.MemberNames.Contains(nameof(LoginDto.Email)))
                {
                    if (string.IsNullOrWhiteSpace(txtUserName.Text))
                    {
                        emailErrors.Add(AppSettings.GetText("Validation_EmptyEmail"));
                    }
                    else
                    {
                        emailErrors.Add(AppSettings.GetText("Validation_InvalidEmail"));
                    }
                }

                // Kiểm tra lỗi Password
                if (result.MemberNames.Contains(nameof(LoginDto.Password)))
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        passwordErrors.Add(AppSettings.GetText("Validation_EmptyPassword"));
                    }
                    else
                    {
                        passwordErrors.Add(AppSettings.GetText("Validation_PasswordTooShort"));
                    }
                }
            }

            // ✅ Hiển thị lỗi email nếu có
            if (emailErrors.Any())
            {
                lblUserError.Text = string.Join("\n", emailErrors);
                lblUserError.Visible = true;
                txtUserName.BorderColor = Color.Red;
                errors.AddRange(emailErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ Email Error: {lblUserError.Text}");
            }

            // ✅ Hiển thị lỗi password nếu có
            if (passwordErrors.Any())
            {
                lblPwdError.Text = string.Join("\n", passwordErrors);
                lblPwdError.Visible = true;
                txtPassword.BorderColor = Color.Red;
                errors.AddRange(passwordErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ Password Error: {lblPwdError.Text}");
            }

            return errors.Count == 0;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // ✅ Validate trước khi gửi API
            if (!ValidateInputs(out var validationErrors))
            {
                // Chỉ hiển thị lỗi trong form
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = AppSettings.GetText("LoginForm_SigningIn");

            try
            {
                var res = await _authService.LoginAsync(txtUserName.Text.Trim(), txtPassword.Text);

                if (res?.Success == true)
                {
                    _authService.SaveUserSession(res, false);

                    // ✅ Tải ngôn ngữ người dùng khi login thành công
                    AppSettings.LoadLanguageSettings();

                    // ✅ NEW: Custom MessageBox Success
                    CustomMessageBox.ShowSuccess(
                        AppSettings.GetText("Message_LoginSuccess"),
                        AppSettings.GetText("Title_Success") ?? "Success"
                    );
                    HomeUser homeForm = new HomeUser(res.UserId);
                    this.Hide();
                    homeForm.Show();
                }
                else if (res?.RequiresVerification == true)
                {
                    // ✅ NEW: Custom MessageBox Info
                    CustomMessageBox.ShowInfo(
                        res.Message,
                        AppSettings.GetText("Title_VerificationRequired") ?? "Verification Required"
                    );

                    // ✅ SỬA: Truyền this (LoginForm) vào VerifyOtpForm
                    var verifyForm = new VerifyOtpForm(this, txtUserName.Text.Trim(), isRegistration: false);
                    verifyForm.Owner = this;
                    this.Hide();
                    verifyForm.ShowDialog();
                }
                else
                {
                    // ✅ NEW: Custom MessageBox Error
                    CustomMessageBox.ShowError(
                        string.IsNullOrWhiteSpace(res?.Message)
                            ? AppSettings.GetText("Message_LoginFailed")
                            : res.Message,
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );

                    lblPwdError.Text = AppSettings.GetText("Validation_WrongCredentials");
                    lblPwdError.Visible = true;
                    txtPassword.BorderColor = Color.OrangeRed;
                }
            }
            catch (Exception ex)
            {
                // ✅ NEW: Custom MessageBox Error for Exception
                CustomMessageBox.ShowError(
                    $"Error: {ex.Message}",
                    AppSettings.GetText("Title_Error") ?? "Error"
                );
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = AppSettings.GetText("LoginForm_BtnLogin");
            }
        }

        private void lblForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // ✅ SỬA: Hide LoginForm rồi Show ForgotPasswordForm
            var forgotForm = new ForgotPasswordForm(this);
            this.Hide();
            forgotForm.Show();
        }

        private void lblRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // ✅ SỬA: Hide LoginForm rồi Show RegisterForm
            var registerForm = new RegisterForm(this);
            this.Hide();
            registerForm.Show();
        }

        private async void btnGoogleLogin_Click(object sender, EventArgs e)
        {
            btnGoogleLogin.Enabled = false;
            btnGoogleLogin.Text = AppSettings.GetText("LoginForm_SigningIn");

            try
            {
                var clientId = ConfigurationManager.AppSettings["GoogleClientId"];
                var clientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)
                    || clientId.Contains("YOUR_GOOGLE"))
                {
                    // ✅ NEW: Custom MessageBox Warning
                    CustomMessageBox.ShowWarning(
                        AppSettings.GetText("Message_GoogleConfigMissing"),
                        AppSettings.GetText("Title_Notification") ?? "Notice"
                    );
                    return;
                }

                var response = await _authService.GoogleLoginAsync(clientId, clientSecret);

                if (response?.Success == true)
                {
                    _authService.SaveUserSession(response, false);

                    // ✅ Tải ngôn ngữ khi Google login thành công
                    AppSettings.LoadLanguageSettings();

                    // ✅ NEW: Kiểm tra xem có cần complete profile không
                    if (response.RequiresProfileCompletion)
                    {
                        // ✅ NEW: Custom MessageBox Info
                        CustomMessageBox.ShowInfo(
                            AppSettings.GetText("Message_CompleteProfileRequired") ??
                            "Please complete your profile to continue.",
                            AppSettings.GetText("Title_Notification") ?? "Notice"
                        );

                        // ✅ Mở EditProfileForm (first-time setup after Google Register)
                        // TODO: Uncomment khi đã tạo EditProfileForm
                        /*
                        var editProfileForm = new EditProfileForm(
                            response.Email,
                            isFirstTimeSetup: true,
                            previousForm: this
                        );
                        editProfileForm.Show();
                        this.Hide();
                        */

                        // ✅ TẠM THỜI: Show message with Custom MessageBox
                        CustomMessageBox.ShowInfo(
                            "🚧 EditProfileForm chưa được tạo.\n\n" +
                            "Bạn sẽ được chuyển đến form này để điền thông tin còn thiếu:\n" +
                            "- Full Name\n" +
                            "- Phone Number\n" +
                            "- Gender\n" +
                            "- Birth Date\n" +
                            "- Address\n" +
                            "- Education Level\n" +
                            "- Company\n" +
                            "- Occupation",
                            "TODO: Complete Profile"
                        );
                    }
                    else
                    {
                        // ✅ NEW: Custom MessageBox Success
                        CustomMessageBox.ShowSuccess(
                            AppSettings.GetText("Message_GoogleSuccess"),
                            AppSettings.GetText("Title_Success") ?? "Success"
                        );

                        // TODO: Mở MainForm
                        /*
                        var mainForm = new MainForm();
                        mainForm.Show();
                        this.Hide();
                        */
                    }
                }
                else
                {
                    // ✅ NEW: Custom MessageBox Error
                    CustomMessageBox.ShowError(
                        response?.Message ?? AppSettings.GetText("Message_LoginFailed"),
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );
                }
            }
            catch (Exception ex)
            {
                // ✅ NEW: Custom MessageBox Error for Exception
                CustomMessageBox.ShowError(
                    $"Error: {ex.Message}",
                    AppSettings.GetText("Title_Error") ?? "Error"
                );
            }
            finally
            {
                btnGoogleLogin.Enabled = true;
                btnGoogleLogin.Text = AppSettings.GetText("LoginForm_GoogleBtn");
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtPassword_TextChanged(object sender, EventArgs e) => ClearErrors();

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}