using System;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Drawing;
using System.IO;
using System.Text.Json;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;

namespace GestPipePowerPonit.Views.Auth
{
    public partial class TermsConditionsForm : Form
    {
        private RegisterForm _registerForm;
        private LoginForm _loginForm;
        private string _userEmail;

        // ✅ Registration data passed from RegisterForm
        private RegisterDto _pendingDto;
        private AuthService _authService;

        public bool RegistrationSucceeded { get; private set; } = false;

        // ✅ Constructor receives DTO and AuthService
        public TermsConditionsForm(RegisterDto pendingDto, AuthService authService, RegisterForm registerForm = null, LoginForm loginForm = null)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            _registerForm = registerForm;
            _loginForm = loginForm;
            _pendingDto = pendingDto;
            _authService = authService;
            _userEmail = pendingDto?.Email ?? "";
        }

        private void TermsConditionsForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Properties.Resources.background;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { this.BackColor = Color.Black; }

            CenterControls();
        }

        private void CenterControls()
        {
            pnlCard.Location = new Point(
                (this.ClientSize.Width - pnlCard.Width) / 2,
                (this.ClientSize.Height - pnlCard.Height) / 2);
        }

        // ✅ Agree radio button changed
        private void rbAgree_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAgree.Checked)
            {
                btnNext.Enabled = true;
                btnNext.Text = "Continue";
            }
        }

        // ✅ Disagree radio button changed
        private void rbDisagree_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDisagree.Checked)
            {
                btnNext.Enabled = true;
                btnNext.Text = "Go Back";
            }
        }

        // ✅ Continue button - handles both Agree and Disagree
        private async void btnNext_Click(object sender, EventArgs e)
        {
            // ✅ Check if Disagree selected
            if (rbDisagree.Checked)
            {
                CustomMessageBox.ShowInfo(
                    "Registration incomplete.\n\nAccepting the terms and conditions is required to create your account. Please review and agree to continue.",
                    "Terms Acceptance Required");

                this.Close();

                if (_registerForm != null && !_registerForm.IsDisposed)
                {
                    _registerForm.Show();
                }
                return;
            }

            // ✅ Check if Agree selected
            if (!rbAgree.Checked)
            {
                CustomMessageBox.ShowWarning(
                    "Please select an option to continue.",
                    "Selection Required");
                return;
            }

            // ✅ User agreed - Save acceptance
            SaveTermsAccepted(true);

            // ✅ If no DTO or AuthService → just close (backward compatibility)
            if (_pendingDto == null || _authService == null)
            {
                this.Close();
                return;
            }

            btnNext.Enabled = false;
            btnNext.Text = "Creating...";

            try
            {
                var res = await _authService.RegisterAsync(_pendingDto);

                if (res?.Success == true)
                {
                    RegistrationSucceeded = true;

                    CustomMessageBox.ShowSuccess(
                        "Account created! Please check your email for OTP verification.",
                        "Success"
                    );

                    // Close Terms and RegisterForm
                    this.Close();

                    if (_registerForm != null && !_registerForm.IsDisposed)
                    {
                        _registerForm.Close();
                    }

                    // ✅ Show VerifyOtp
                    var verifyForm = new VerifyOtpForm(_loginForm, _pendingDto.Email, isRegistration: true);
                    verifyForm.ShowDialog();
                }
                else
                {
                    // ✅ Registration failed
                    string errorMsg = res?.Message ?? "Registration failed. Please try again.";
                    CustomMessageBox.ShowError(errorMsg, "Error");

                    // Return to RegisterForm
                    this.Close();

                    if (_registerForm != null && !_registerForm.IsDisposed)
                    {
                        _registerForm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during registration: {ex.Message}");
                CustomMessageBox.ShowError($"Error: {ex.Message}", "Error");

                this.Close();

                if (_registerForm != null && !_registerForm.IsDisposed)
                {
                    _registerForm.Show();
                }
            }
            finally
            {
                try
                {
                    btnNext.Enabled = true;
                    btnNext.Text = "Continue";
                }
                catch { }
            }
        }

        private void TermsConditionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If user closes without selecting anything, show RegisterForm
            if (!rbAgree.Checked && !rbDisagree.Checked && _registerForm != null && !_registerForm.IsDisposed)
            {
                _registerForm.Show();
            }
        }

        private void SaveTermsAccepted(bool accepted)
        {
            try
            {
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GestPipePowerPonit",
                    "AppConfig.json"
                );

                string directory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var config = new { Language = "EN", TermsAccepted = accepted };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving config: {ex.Message}");
            }
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}