using GestPipePowerPonit;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit.Views.Auth
{
    public partial class RegisterForm : Form
    {
        private AuthService _authService;
        private LoginForm _loginForm;

        public RegisterForm(LoginForm loginForm = null)
        {
            InitializeComponent();
            _authService = new AuthService();
            _loginForm = loginForm;

            //this.Size = new Size(1366, 786);
            //this.MinimumSize = new Size(1366, 786);
            //this.MaximumSize = new Size(1366, 786);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.DoubleBuffered = true;
            this.Resize += (s, e) => CenterControls();

            AppSettings.CurrentLanguage = "EN";
        }

        private void RegisterForm_Resize(object sender, EventArgs e) => CenterControls();

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackgroundImage = Properties.Resources.background;
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { this.BackColor = Color.Black; }

            LoadLanguage();
            CenterControls();

            cmbGender.SelectedIndex = 0;
            cmbAddress.SelectedIndex = 0;
            cmbEdu.SelectedIndex = 0;
            cmbOccupation.SelectedIndex = 0;  // ✅ Thêm dòng này
            dtpBirthDate.Value = DateTime.Now.AddYears(-18);
        }

        private void LoadLanguage()
        {
            lblTitle.Text = AppSettings.GetText("RegisterForm_Title");
            btnRegister.Text = AppSettings.GetText("RegisterForm_BtnRegister");
            lnkLogin.Text = AppSettings.GetText("RegisterForm_Login");
            lnkLogin.LinkBehavior = LinkBehavior.NeverUnderline;
            btnGoogle.Text = AppSettings.GetText("RegisterForm_GoogleBtn");

            lblEmail.Text = AppSettings.GetText("RegisterForm_Lbl_Email");
            lblFullName.Text = AppSettings.GetText("RegisterForm_Lbl_FullName");
            lblPassword.Text = AppSettings.GetText("RegisterForm_Lbl_Password");
            lblConfirmPassword.Text = AppSettings.GetText("RegisterForm_Lbl_ConfirmPassword");
            lblPhoneNumber.Text = AppSettings.GetText("RegisterForm_Lbl_Phone");
            lblGender.Text = AppSettings.GetText("RegisterForm_Lbl_Gender");
            lblBirthDate.Text = AppSettings.GetText("RegisterForm_Lbl_BirthDate");
            lblAddress.Text = AppSettings.GetText("RegisterForm_Lbl_Address");
            lblEdu.Text = AppSettings.GetText("RegisterForm_Lbl_Edu");
            lblCompany.Text = AppSettings.GetText("RegisterForm_Lbl_Company");
            lblOccupation.Text = AppSettings.GetText("RegisterForm_Lbl_Occupation");

            txtEmail.PlaceholderText = AppSettings.GetText("RegisterForm_Placeholder_Email");
            txtFullName.PlaceholderText = AppSettings.GetText("RegisterForm_Placeholder_FullName");
            txtPassword.PlaceholderText = AppSettings.GetText("RegisterForm_Placeholder_Password");
            txtPhoneNumber.PlaceholderText = AppSettings.GetText("RegisterForm_Placeholder_Phone");
            txtCompany.PlaceholderText = AppSettings.GetText("RegisterForm_Placeholder_Company");
            //txtOccupation.PlaceholderText = AppSettings.GetText("RegisterForm_Placeholder_Occupation");
        }
        private void ClearErrors()
        {
            lblEmailError.Visible = false;
            lblFullNameError.Visible = false;
            lblPasswordError.Visible = false;
            lblConfirmPasswordError.Visible = false;
            lblPhoneNumberError.Visible = false;
            lblGenderError.Visible = false;
            lblBirthDateError.Visible = false;
            lblAddressError.Visible = false;
            lblEduError.Visible = false;
            lblCompanyError.Visible = false;
            lblOccupationError.Visible = false;

            txtEmail.BorderColor = Color.FromArgb(220, 220, 220);
            txtFullName.BorderColor = Color.FromArgb(220, 220, 220);
            txtPassword.BorderColor = Color.FromArgb(220, 220, 220);
            txtConfirmPassword.BorderColor = Color.FromArgb(220, 220, 220);
            txtPhoneNumber.BorderColor = Color.FromArgb(220, 220, 220);
            cmbGender.BorderColor = Color.FromArgb(220, 220, 220);
            dtpBirthDate.BorderColor = Color.FromArgb(220, 220, 220);
            cmbAddress.BorderColor = Color.FromArgb(220, 220, 220);
            cmbEdu.BorderColor = Color.FromArgb(220, 220, 220);
            txtCompany.BorderColor = Color.FromArgb(220, 220, 220);
            cmbOccupation.BorderColor = Color.FromArgb(220, 220, 220);  // ✅ Thay đổi từ txtOccupation
            txtOccupationOther.BorderColor = Color.FromArgb(220, 220, 220);  // ✅ Thêm mới
        }
        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();

            var registerDto = new RegisterDto
            {
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text,
                ReEnterPassword = txtConfirmPassword.Text,
                FullName = txtFullName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text?.Trim() ?? "",
                Gender = cmbGender.SelectedIndex > 0 ? cmbGender.SelectedItem.ToString() : "",
                DateOfBirth = dtpBirthDate.Value,
                Address = cmbAddress.SelectedIndex > 0 ? cmbAddress.SelectedItem.ToString() : "",
                EducationLevel = cmbEdu.SelectedIndex > 0 ? cmbEdu.SelectedItem.ToString() : "",
                Company = txtCompany.Text?.Trim() ?? "",
                Occupation = GetOccupationValue() ?? ""
            };

            var context = new ValidationContext(registerDto);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(registerDto, context, validationResults, true);

            System.Diagnostics.Debug.WriteLine($"[Validation] Valid={isValid}, Errors={validationResults.Count}");

            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    switch (memberName)
                    {
                        case nameof(RegisterDto.Email):
                            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                                lblEmailError.Text = AppSettings.GetText("Validation_EmailRequired");
                            else
                                lblEmailError.Text = AppSettings.GetText("Validation_InvalidEmail");
                            lblEmailError.Visible = true;
                            txtEmail.BorderColor = Color.OrangeRed;
                            errors.Add(lblEmailError.Text);
                            break;

                        case nameof(RegisterDto.FullName):
                            lblFullNameError.Text = AppSettings.GetText("Validation_FullNameRequired");
                            lblFullNameError.Visible = true;
                            txtFullName.BorderColor = Color.OrangeRed;
                            errors.Add(lblFullNameError.Text);
                            break;

                        case nameof(RegisterDto.Password):
                            if (string.IsNullOrWhiteSpace(txtPassword.Text))
                                lblPasswordError.Text = AppSettings.GetText("Validation_PasswordRequired");
                            else
                                lblPasswordError.Text = AppSettings.GetText("Validation_PasswordTooShort");
                            lblPasswordError.Visible = true;
                            txtPassword.BorderColor = Color.OrangeRed;
                            errors.Add(lblPasswordError.Text);
                            break;

                        case nameof(RegisterDto.ReEnterPassword):
                            lblConfirmPasswordError.Text = AppSettings.GetText("Validation_ConfirmPasswordRequired");
                            lblConfirmPasswordError.Visible = true;
                            txtConfirmPassword.BorderColor = Color.OrangeRed;
                            errors.Add(lblConfirmPasswordError.Text);
                            break;

                        //case nameof(RegisterDto.PhoneNumber):
                        //    if (result.ErrorMessage.Contains("digits"))
                        //        lblPhoneNumberError.Text = AppSettings.GetText("Validation_PhoneOnlyDigits");
                        //    else if (result.ErrorMessage.Contains("10"))
                        //        lblPhoneNumberError.Text = AppSettings.GetText("Validation_Phone10Digits");
                        //    else if (result.ErrorMessage.Contains("0"))
                        //        lblPhoneNumberError.Text = AppSettings.GetText("Validation_PhoneStartWith0");
                        //    else
                        //        lblPhoneNumberError.Text = AppSettings.GetText("Validation_PhoneOnlyDigits");

                        //    lblPhoneNumberError.Visible = true;
                        //    txtPhoneNumber.BorderColor = Color.OrangeRed;
                        //    errors.Add(lblPhoneNumberError.Text);
                        //    break;
                        //case nameof(RegisterDto.PhoneNumber):
                        //    // nếu muốn tách Required riêng thì check thêm:
                        //    if (string.IsNullOrWhiteSpace(registerDto.PhoneNumber))
                        //        lblPhoneNumberError.Text = AppSettings.GetText("Validation_PhoneRequired");
                        //    else
                        //        lblPhoneNumberError.Text = AppSettings.GetText("Validation_PhoneInvalidFormat");

                        //    lblPhoneNumberError.Visible = true;
                        //    txtPhoneNumber.BorderColor = Color.OrangeRed;
                        //    errors.Add(lblPhoneNumberError.Text);
                        //    break;
                        case nameof(RegisterDto.PhoneNumber):
                            {
                                var phone = registerDto.PhoneNumber?.Trim() ?? "";

                                if (string.IsNullOrWhiteSpace(phone))
                                {
                                    // Bắt buộc nhập
                                    lblPhoneNumberError.Text ="Phone number is required";
                                }
                                else if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$"))
                                {
                                    // Chỉ cho phép số
                                    lblPhoneNumberError.Text = "Phone must contain only digits";
                                }
                                else if (phone.Length < 10 || phone.Length > 11)
                                {
                                    // Độ dài 10–11 ký tự
                                    lblPhoneNumberError.Text ="Phone must be 10–11 digits";
                                }
                                else if (!phone.StartsWith("0"))
                                {
                                    // Bắt đầu bằng 0
                                    lblPhoneNumberError.Text ="Phone must start with 0";
                                }
                                else
                                {
                                    // fallback nếu format sai kiểu khác
                                    lblPhoneNumberError.Text ="Invalid phone number format";
                                }

                                lblPhoneNumberError.Visible = true;
                                txtPhoneNumber.BorderColor = Color.OrangeRed;
                                errors.Add(lblPhoneNumberError.Text);
                                break;
                            }


                        case nameof(RegisterDto.Gender):
                            lblGenderError.Text = AppSettings.GetText("Validation_SelectGender");
                            lblGenderError.Visible = true;
                            cmbGender.BorderColor = Color.OrangeRed;
                            errors.Add(lblGenderError.Text);
                            break;

                        case nameof(RegisterDto.DateOfBirth):
                            lblBirthDateError.Text = AppSettings.GetText("Validation_DateOfBirthRequired");
                            lblBirthDateError.Visible = true;
                            dtpBirthDate.BorderColor = Color.OrangeRed;
                            errors.Add(lblBirthDateError.Text);
                            break;

                        case nameof(RegisterDto.Address):
                            lblAddressError.Text = AppSettings.GetText("Validation_SelectAddress");
                            lblAddressError.Visible = true;
                            cmbAddress.BorderColor = Color.OrangeRed;
                            errors.Add(lblAddressError.Text);
                            break;

                        case nameof(RegisterDto.EducationLevel):
                            lblEduError.Text = AppSettings.GetText("Validation_SelectEdu");
                            lblEduError.Visible = true;
                            cmbEdu.BorderColor = Color.OrangeRed;
                            errors.Add(lblEduError.Text);
                            break;

                        case nameof(RegisterDto.Company):
                            lblCompanyError.Text = AppSettings.GetText("Validation_CompanyRequired");
                            lblCompanyError.Visible = true;
                            txtCompany.BorderColor = Color.OrangeRed;
                            errors.Add(lblCompanyError.Text);
                            break;

                        case nameof(RegisterDto.Occupation):
                            lblOccupationError.Text = AppSettings.GetText("Validation_OccupationRequired");
                            lblOccupationError.Visible = true;
                            cmbOccupation.BorderColor = Color.OrangeRed;
                            errors.Add(lblOccupationError.Text);
                            break;
                    }
                }
            }

            // ✅ CUSTOM VALIDATION: Password match
            if (!string.IsNullOrEmpty(txtPassword.Text) &&
                !string.IsNullOrEmpty(txtConfirmPassword.Text) &&
                txtPassword.Text != txtConfirmPassword.Text)
            {
                string errorMsg = AppSettings.GetText("Validation_PasswordMismatch");
                lblConfirmPasswordError.Text = errorMsg;
                lblConfirmPasswordError.Visible = true;
                txtConfirmPassword.BorderColor = Color.OrangeRed;
                errors.Add(errorMsg);
            }

            // ✅ CUSTOM VALIDATION: Occupation "Other" field
            if (cmbOccupation.SelectedIndex == 0)
            {
                lblOccupationError.Text = AppSettings.GetText("Validation_OccupationRequired") ?? "Please select an occupation";
                lblOccupationError.Visible = true;
                cmbOccupation.BorderColor = Color.OrangeRed;
                errors.Add(lblOccupationError.Text);
            }
            else if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1) // "Other"
            {
                string customOccupation = txtOccupationOther.Text?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(customOccupation))
                {
                    lblOccupationError.Text = AppSettings.GetText("Validation_OccupationSpecifyRequired") ?? "Please specify your occupation";
                    lblOccupationError.Visible = true;
                    txtOccupationOther.BorderColor = Color.OrangeRed;
                    errors.Add(lblOccupationError.Text);
                }
                else if (customOccupation.Length < 2 || customOccupation.Length > 50)
                {
                    lblOccupationError.Text = AppSettings.GetText("Validation_OccupationLength") ?? "Occupation must be 2-50 characters";
                    lblOccupationError.Visible = true;
                    txtOccupationOther.BorderColor = Color.OrangeRed;
                    errors.Add(lblOccupationError.Text);
                }
            }

            return errors.Count == 0;
        }

        // ✅ CORRECT FLOW: Validate → Terms → Agree → Create Account + Send OTP → VerifyOtp
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // ✅ Step 1: Validate form
            if (!ValidateInputs(out var validationErrors))
            {
                return;
            }
            string email = txtEmail.Text.Trim();

            // ✅ Step 2: CHECK EMAIL UNIQUENESS (MỚI THÊM)
            btnRegister.Enabled = false;
            btnRegister.Text = "Checking...";

            try
            {
                bool isAvailable = await IsEmailAvailableAsync(email);

                if (!isAvailable)
                {
                    // ❌ Email đã tồn tại - DỪNG NGAY, KHÔNG SHOW TERMS
                    //lblEmailError.Text = "This email is already registered. Please use another email or login.";
                    //lblEmailError.Visible = true;
                    //txtEmail.BorderColor = Color.OrangeRed;
                    //txtEmail.Focus();
                    //return; // ← DỪNG TẠI ĐÂY

                    lblEmailError.Text =
                AppSettings.GetText("Validation_EmailAlreadyExists")
                ?? "This email is already registered. Please use another email or login.";

                    lblEmailError.Visible = true;
                    txtEmail.BorderColor = Color.OrangeRed;
                    txtEmail.Focus();
                    return; // DỪNG HẲN
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[btnRegister_Click] Email check error: {ex.Message}");
                CustomMessageBox.ShowError(
            AppSettings.GetText("Message_EmailCheckError")
            ?? "Cannot verify your email at the moment. Please check your network connection and try again.",
            AppSettings.GetText("Title_Error") ?? "Error"
        );
                return; // DỪNG HẲN
            }
            finally
            {
                btnRegister.Enabled = true;
                btnRegister.Text = AppSettings.GetText("RegisterForm_BtnRegister");
            }
            // ✅ Step 2: Prepare DTO (chưa gọi API)
            var dto = new RegisterDto
            {
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text,
                ReEnterPassword = txtConfirmPassword.Text,
                FullName = txtFullName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text?.Trim(),
                Gender = cmbGender.SelectedItem?.ToString() ?? "",
                DateOfBirth = dtpBirthDate.Value,
                Address = cmbAddress.SelectedItem?.ToString() ?? "",
                EducationLevel = cmbEdu.SelectedItem?.ToString() ?? "",
                Company = txtCompany.Text?.Trim(),
                Occupation = GetOccupationValue()
            };

            this.Hide();

            var termsForm = new TermsConditionsForm(dto, _authService, this, _loginForm);
            termsForm.ShowDialog();

            // ✅ If user disagreed or closed Terms → show Register again
            if (!termsForm.RegistrationSucceeded && !this.IsDisposed)
            {
                this.Show();
            }
            else
            {
                // ✅ Registration succeeded → close Register
                if (!this.IsDisposed)
                {
                    this.Close();
                }
            }
        }

        private async void btnGoogle_Click(object sender, EventArgs e)
        {
            btnGoogle.Enabled = false;
            btnGoogle.Text = AppSettings.GetText("RegisterForm_SigningIn") ?? "Signing in...";

            try
            {
                var clientId = System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
                var clientSecret = System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"];

                if (string.IsNullOrEmpty(clientId) || clientId.Contains("YOUR_GOOGLE"))
                {
                    CustomMessageBox.ShowWarning(
                        AppSettings.GetText("Message_GoogleConfigMissing") ?? "Google Client ID not configured",
                        AppSettings.GetText("Title_Notification"));
                    return;
                }

                var response = await _authService.GoogleLoginAsync(clientId, clientSecret);

                if (response?.Success == true)
                {
                    _authService.SaveUserSession(response, false);
                    AppSettings.LoadLanguageSettings();

                    if (response.RequiresProfileCompletion)
                    {
                        CustomMessageBox.ShowInfo(
                            AppSettings.GetText("Message_CompleteProfileRequired") ??
                            "Please complete your profile to continue.",
                            AppSettings.GetText("Title_Notification")
                        );
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
                        CustomMessageBox.ShowSuccess(
                            AppSettings.GetText("Message_GoogleSuccess") ?? "Login successful!",
                            "Success");
                        HomeUser homeForm = new HomeUser(response.UserId);
                        this.Hide();
                        homeForm.Show();
                    }
                }
                else
                {
                    CustomMessageBox.ShowError(
                        response?.Message ?? AppSettings.GetText("Message_LoginFailed"),
                        AppSettings.GetText("Title_Error")
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}",
                    AppSettings.GetText("Title_Error"));
            }
            finally
            {
                btnGoogle.Enabled = true;
                btnGoogle.Text = AppSettings.GetText("RegisterForm_GoogleBtn") ?? "Continue with Google";
            }
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_loginForm != null && !_loginForm.IsDisposed)
            {
                _loginForm.Show();
            }
            this.Close();
        }

        private void CenterControls()
        {
            pnlCard.Location = new Point(
                (this.ClientSize.Width - pnlCard.Width) / 2,
                (this.ClientSize.Height - pnlCard.Height) / 2);
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = txtPassword.PasswordChar == '\0' ? '●' : '\0';
        }

        private void txtConfirmPassword_IconRightClick(object sender, EventArgs e)
        {
            txtConfirmPassword.PasswordChar = txtConfirmPassword.PasswordChar == '\0' ? '●' : '\0';
        }

        private void cmbOccupation_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Show txtOccupationOther nếu chọn "Other (specify below)"
            if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1)
            {
                txtOccupationOther.Visible = true;
                txtOccupationOther.Focus();
            }
            else
            {
                txtOccupationOther.Visible = false;
                txtOccupationOther.Text = "";
            }

            ClearErrors();
        }

        // ✅ TextBox OccupationOther event
        private void txtOccupationOther_TextChanged(object sender, EventArgs e)
        {
            ClearErrors();
        }
        private string GetOccupationValue()
        {
            if (cmbOccupation.SelectedIndex <= 0)
                return "";

            // Nếu chọn "Other (specify below)"
            if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1)
            {
                return txtOccupationOther.Text?.Trim() ?? "";
            }

            return cmbOccupation.SelectedItem?.ToString() ?? "";
        }
        /// <summary>
        /// ✅ Check email uniqueness trước khi show Terms
        /// </summary>
        private async Task<bool> IsEmailAvailableAsync(string email)
        {
            try
            {
                // true = email đã tồn tại trong hệ thống
                bool exists = await _authService.CheckEmailExistsAsync(email);
                return !exists; // true = available, false = đã tồn tại
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IsEmailAvailableAsync] Error: {ex.Message}");
                // ✅ Không nuốt lỗi nữa, để btnRegister_Click quyết định
                throw;
            }
        }
        // ===== TextChanged Events =====
        private void txtEmail_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtFullName_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtPassword_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtConfirmPassword_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtPhoneNumber_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e) => ClearErrors();
        private void dtpBirthDate_ValueChanged(object sender, EventArgs e) => ClearErrors();
        private void cmbAddress_SelectedIndexChanged(object sender, EventArgs e) => ClearErrors();
        private void cmbEdu_SelectedIndexChanged(object sender, EventArgs e) => ClearErrors();
        private void txtCompany_TextChanged(object sender, EventArgs e) => ClearErrors();
        //private void txtOccupation_TextChanged(object sender, EventArgs e) => ClearErrors();

        public string GetUserEmail()
        {
            return txtEmail.Text.Trim();
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}