using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
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

            lblTitle.Text = AppSettings.GetText("LoginForm_Title");
            btnLogin.Text = AppSettings.GetText("LoginForm_BtnLogin");
            lblForgotPassword.Text = AppSettings.GetText("LoginForm_ForgotPassword");
            lblNoAccount.Text = AppSettings.GetText("LoginForm_NoAccount");
            lblRegister.Text = AppSettings.GetText("LoginForm_Register");
            lblRegister.LinkBehavior = LinkBehavior.NeverUnderline;
            lblForgotPassword.LinkBehavior = LinkBehavior.NeverUnderline;
            btnGoogleLogin.Text = AppSettings.GetText("LoginForm_GoogleBtn");
            chkRememberMe.Text = AppSettings.GetText("LoginForm_RememberMe") ?? "Remember Me";

            // ✅ LOAD SAVED EMAIL NẾU CÓ
            LoadRememberedEmail();

            CenterControls();
        }

        // ✅ NEW METHOD: Load email đã lưu
        private void LoadRememberedEmail()
        {
            try
            {
                bool rememberMe = Properties.Settings.Default.RememberMe;

                if (rememberMe)
                {
                    string savedEmail = Properties.Settings.Default.SavedEmail ?? "";

                    if (!string.IsNullOrEmpty(savedEmail))
                    {
                        txtUserName.Text = savedEmail;
                        chkRememberMe.Checked = true;

                        // ✅ Focus vào password field để user chỉ cần nhập password
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadRememberedEmail] Error: {ex.Message}");
            }
        }

        // ✅ NEW METHOD: Lưu email khi Remember Me được check
        private void SaveEmailIfNeeded(string email)
        {
            try
            {
                if (chkRememberMe.Checked)
                {
                    Properties.Settings.Default.SavedEmail = email;
                    Properties.Settings.Default.RememberMe = true;
                }
                else
                {
                    // Clear saved email
                    Properties.Settings.Default.SavedEmail = "";
                    Properties.Settings.Default.RememberMe = false;
                }

                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveEmailIfNeeded] Error: {ex.Message}");
            }
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

            var loginDto = new LoginDto
            {
                Email = txtUserName.Text.Trim(),
                Password = txtPassword.Text
            };

            var context = new ValidationContext(loginDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(loginDto, context, validationResults, true);

            System.Diagnostics.Debug.WriteLine($"[Validation] Valid={isValid}, Errors={validationResults.Count}");

            var emailErrors = new List<string>();
            var passwordErrors = new List<string>();

            foreach (var result in validationResults)
            {
                System.Diagnostics.Debug.WriteLine($"  └─ Member: {string.Join(", ", result.MemberNames)}");

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

            if (emailErrors.Any())
            {
                lblUserError.Text = string.Join("\n", emailErrors);
                lblUserError.Visible = true;
                txtUserName.BorderColor = Color.Red;
                errors.AddRange(emailErrors);

                System.Diagnostics.Debug.WriteLine($"  └─ Email Error: {lblUserError.Text}");
            }

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
            if (!ValidateInputs(out var validationErrors))
            {
                return;
            }


            btnLogin.Enabled = false;
            btnLogin.Text = AppSettings.GetText("LoginForm_SigningIn");


            try
            {
                string email = txtUserName.Text.Trim();
                string password = txtPassword.Text;


                var res = await _authService.LoginAsync(email, password);


                if (res?.Success == true)
                {
                    // ✅ SỬA: Truyền chkRememberMe.Checked thay vì hard-coded false
                    _authService.SaveUserSession(res, chkRememberMe.Checked);


                    // ✅ LƯU EMAIL NẾU REMEMBER ME (KHÔNG LƯU PASSWORD)
                    SaveEmailIfNeeded(email);


                    AppSettings.LoadLanguageSettings();


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
                    CustomMessageBox.ShowInfo(
                        res.Message,
                        AppSettings.GetText("Title_VerificationRequired") ?? "Verification Required"
                    );


                    var verifyForm = new VerifyOtpForm(this, email, isRegistration: false);
                    verifyForm.Owner = this;
                    this.Hide();
                    verifyForm.ShowDialog();
                }
                else
                {
                    var errorMsg = string.IsNullOrWhiteSpace(res?.Message)
                        ? AppSettings.GetText("Message_LoginFailed")
                        : res.Message;


                    CustomMessageBox.ShowError(
                        errorMsg,
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );


                    // Nếu là thông báo Google thì coi như info, không ghi "Sai mật khẩu"
                    if (errorMsg.Contains("Google"))
                    {
                        //lblPwdError.Text = errorMsg;
                        //txtPassword.BorderColor = Color.Goldenrod; // nhẹ hơn
                    }
                    else
                    {
                        lblPwdError.Text = AppSettings.GetText("Validation_WrongCredentials");
                        txtPassword.BorderColor = Color.OrangeRed;
                    }


                    lblPwdError.Visible = true;
                }
            }
            catch (Exception ex)
            {
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
            var forgotForm = new ForgotPasswordForm(this);
            this.Hide();
            forgotForm.Show();
        }

        private void lblRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
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
                    CustomMessageBox.ShowWarning(
                        AppSettings.GetText("Message_GoogleConfigMissing"),
                        AppSettings.GetText("Title_Notification") ?? "Notice"
                    );
                    return;
                }

                var response = await _authService.GoogleLoginAsync(clientId, clientSecret);

                if (response?.Success == true)
                {
                    // ✅ Lưu session (không lưu remember me cho Google login)
                    _authService.SaveUserSession(response, false);

                    AppSettings.LoadLanguageSettings();

                    if (response.RequiresProfileCompletion)
                    {
                        CustomMessageBox.ShowInfo(
                            AppSettings.GetText("Message_CompleteProfileRequired") ??
                            "Please complete your profile to continue.",
                            AppSettings.GetText("Title_Notification") ?? "Notice"
                        );

                        // TODO: Chuyển đến EditProfileForm khi đã tạo
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

                        // ✅ SỬA: Vẫn chuyển vào HomeUser ngay cả khi cần complete profile
                        HomeUser homeForm = new HomeUser(response.UserId);
                        this.Hide();
                        homeForm.Show();
                    }
                    else
                    {
                        // ✅ Profile đã đầy đủ - chuyển vào HomeUser
                        CustomMessageBox.ShowSuccess(
                            AppSettings.GetText("Message_GoogleSuccess"),
                            AppSettings.GetText("Title_Success") ?? "Success"
                        );

                        HomeUser homeForm = new HomeUser(response.UserId);
                        this.Hide();
                        homeForm.Show();
                    }
                }
                else
                {
                    CustomMessageBox.ShowError(
                        response?.Message ?? AppSettings.GetText("Message_LoginFailed"),
                        AppSettings.GetText("Title_Error") ?? "Error"
                    );
                }
            }
            catch (Exception ex)
            {
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