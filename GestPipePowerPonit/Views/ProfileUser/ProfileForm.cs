using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Views.Auth;

namespace GestPipePowerPonit.Views.Profile
{
    public partial class ProfileForm : Form
    {
        private readonly Form _parentForm;
        private readonly AuthService _authService;
        private readonly ProfileService _profileService;
        private readonly ApiClient _apiClient;
        private readonly string _userId;
        private UserProfileDto _currentProfile;
        private UserResponseDto _currentUser;
        private bool _isEditMode = false;
        private HomeUser _homeForm;
        private string _currentCultureCode;

        public ProfileForm(string userId, HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += ProfileForm_Load;

            _authService = new AuthService();
            _profileService = new ProfileService();
            _apiClient = new ApiClient("https://localhost:7219");
            _userId = userId;
            _homeForm = homeForm;

            InitializeSidebarEvents();
        }

        private void InitializeSidebarEvents()
        {
            //btnHome.Click += BtnHome_Click;
            //btnGestureControl.Click += BtnGestureControl_Click;
            //btnPresentation.Click += BtnPresentation_Click;
            btnCustomGesture.Click += BtnCustomGesture_Click;
            btnLanguageEN.Click += async (s, e) => await ChangeLanguageAsync("en-US");
            btnLanguageVN.Click += async (s, e) => await ChangeLanguageAsync("vi-VN");

            // ✅ GẮN SỰ KIỆN LOGOUT
            btnLogout.Click += btnLogout_Click;

            CultureManager.CultureChanged += (s, e) =>
            {
                _currentCultureCode = CultureManager.CurrentCultureCode; // cập nhật lại code
                ResourceHelper.SetCulture(_currentCultureCode, this);    // set lại UI
                ApplyLanguage();                                         // gán lại text toàn bộ UI
            };
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            _homeForm?.Show();
            this.Hide();
        }

        private void BtnGestureControl_Click(object sender, EventArgs e)
        {
            try
            {
                ListDefaultGestureForm defaultGesture = new ListDefaultGestureForm(_homeForm);
                defaultGesture.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}", Properties.Resources.Title_Error);
            }
        }

        private void BtnPresentation_Click(object sender, EventArgs e)
        {
            try
            {
                PresentationForm form1 = new PresentationForm(_homeForm);
                form1.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}", Properties.Resources.Title_Error);
            }
        }

        private void BtnCustomGesture_Click(object sender, EventArgs e)
        {
            try
            {
                FormUserGesture usergestureForm = new FormUserGesture(_homeForm);
                usergestureForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}", Properties.Resources.Title_Error);
            }
        }

        private async void ProfileForm_Load(object sender, EventArgs e)
        {
            await LoadUserLanguage();
            ApplyLanguage();
            SetReadOnlyMode();
            await LoadProfileData();
        }

        private async Task LoadUserLanguage()
        {
            try
            {
                var user = await _apiClient.GetUserAsync(_userId);

                _currentCultureCode = (user != null && !string.IsNullOrWhiteSpace(user.UiLanguage))
                    ? user.UiLanguage
                    : "en-US";

                CultureManager.CurrentCultureCode = _currentCultureCode;
                //ResourceHelper.SetCulture(_currentCultureCode, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadUserLanguage] Error: {ex.Message}");
            }
        }

        private async Task ChangeLanguageAsync(string cultureCode)
        {
            try
            {
                CultureManager.CurrentCultureCode = cultureCode;
                ResourceHelper.SetCulture(cultureCode, this);

                await _apiClient.SetUserLanguageAsync(_userId, cultureCode);

                CustomMessageBox.ShowSuccess(
                    Properties.Resources.Message_ChangeLanguageSuccess,
                    Properties.Resources.Title_Success
                );
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError(
                    Properties.Resources.Message_ChangeLanguageFailed,
                    Properties.Resources.Title_Error
                );
            }
        }

        private void ApplyLanguage()
        {
            try
            {
                ResourceHelper.SetCulture(_currentCultureCode, this);
                // Sidebar
                btnHome.Text = Properties.Resources.Btn_Home;
                btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
                btnInstruction.Text = Properties.Resources.Btn_Instruction;
                btnPresentation.Text = Properties.Resources.Btn_Present;
                btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;

                // Profile fields               
                lblEmail.Text = Properties.Resources.ProfileForm_Email;
                lblPhone.Text = Properties.Resources.ProfileForm_Phone;
                lblGender.Text = Properties.Resources.ProfileForm_Gender;
                lblBirthDate.Text = Properties.Resources.ProfileForm_BirthDate;
                lblAddress.Text = Properties.Resources.ProfileForm_Address;
                lblEducation.Text = Properties.Resources.ProfileForm_Education;
                lblCompany.Text = Properties.Resources.ProfileForm_Company;
                lblOccupation.Text = Properties.Resources.ProfileForm_Occupation;

                // Buttons
                btnEdit.Text = Properties.Resources.ProfileForm_BtnEdit;
                btnSave.Text = Properties.Resources.ProfileForm_BtnSave;
                btnCancel.Text = Properties.Resources.ProfileForm_BtnCancel;
                btnChangePassword.Text = Properties.Resources.ProfileForm_BtnChangePassword;

                // Gender combo
                cmbGender.Items.Clear();
                cmbGender.Items.AddRange(new object[] {
                    Properties.Resources.Gender_Male,
                    Properties.Resources.Gender_Female,
                    Properties.Resources.Gender_Other
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApplyLanguage] Error: {ex.Message}");
            }
        }

        private async Task LoadProfileData()
        {
            try
            {
                lblLoading.Visible = true;
                lblLoading.Text = Properties.Resources.ProfileForm_Loading;

                var response = await _profileService.GetProfileAsync(_userId);

                if (response?.Success == true && response.Data != null)
                {
                    _currentProfile = response.Data.Profile;
                    _currentUser = response.Data.User;
                    PopulateFormFields();
                    await LoadAvatar();

                    // ✅ Check profile completeness
                    CheckProfileCompleteness();
                }
                else
                {
                    CustomMessageBox.ShowError(
                        response?.Message ?? Properties.Resources.Message_LoadProfileFailed,
                        Properties.Resources.Title_Error
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}", Properties.Resources.Title_Error);
            }
            finally
            {
                lblLoading.Visible = false;
            }
        }

        /// <summary>
        /// ✅ Check profile completeness and show banner if needed
        /// </summary>
        private void CheckProfileCompleteness()
        {
            if (_currentProfile == null) return;

            int total = 7;
            int completed = 0;

            if (!string.IsNullOrWhiteSpace(_currentProfile.PhoneNumber)) completed++;
            if (!string.IsNullOrWhiteSpace(_currentProfile.Gender)) completed++;
            if (_currentProfile.BirthDate.HasValue) completed++;
            if (!string.IsNullOrWhiteSpace(_currentProfile.Address)) completed++;
            if (!string.IsNullOrWhiteSpace(_currentProfile.EducationLevel)) completed++;
            if (!string.IsNullOrWhiteSpace(_currentProfile.Occupation)) completed++;
            if (!string.IsNullOrWhiteSpace(_currentProfile.Company)) completed++;

            int percentage = (completed * 100) / total;

            Console.WriteLine($"[Profile] Completion: {percentage}% ({completed}/{total})");

            if (percentage < 70 && panelBanner != null)
            {
                lblBannerMessage.Text = $"Your profile is {percentage}% complete. Add more info for a better experience!";
                panelBanner.Visible = true;
                panelBanner.BringToFront();
            }
        }

        /// <summary>
        /// ✅ Populate form fields with data
        /// </summary>
        private void PopulateFormFields()
        {
            if (_currentProfile == null || _currentUser == null) return;

            // FullName
            lblFullNameValue.Text = _currentProfile.FullName ?? "User";
            txtFullName.Text = _currentProfile.FullName ?? "";

            // Email
            lblEmailValue.Text = _currentUser.Email ?? "-";
            txtEmail.Text = _currentUser.Email ?? "";

            // ✅ Optional fields with gray color
            SetFieldValue(lblPhoneValue, txtPhone, _currentProfile.PhoneNumber, "Not added");
            SetFieldValue(lblGenderValue, null, _currentProfile.Gender, "Not specified");
            SetFieldValue(lblBirthDateValue, null, _currentProfile.BirthDate?.ToString("MMMM d, yyyy"), "Not set");
            SetFieldValue(lblAddressValue, null, _currentProfile.Address, "Not provided");
            SetFieldValue(lblEducationValue, null, _currentProfile.EducationLevel, "Not added");
            SetFieldValue(lblCompanyValue, txtCompany, _currentProfile.Company, "Not specified");
            SetFieldValue(lblOccupationValue, null, _currentProfile.Occupation, "Not added");

            // ✅ Gender ComboBox
            if (!string.IsNullOrEmpty(_currentProfile.Gender))
            {
                string genderKey = _currentProfile.Gender.ToLower();
                switch (genderKey)
                {
                    case "male":
                        lblGenderValue.Text = Properties.Resources.Gender_Male;
                        lblGenderValue.ForeColor = Color.White;
                        cmbGender.SelectedItem = Properties.Resources.Gender_Male;
                        break;
                    case "female":
                        lblGenderValue.Text = Properties.Resources.Gender_Female;
                        lblGenderValue.ForeColor = Color.White;
                        cmbGender.SelectedItem = Properties.Resources.Gender_Female;
                        break;
                    case "other":
                        lblGenderValue.Text = Properties.Resources.Gender_Other;
                        lblGenderValue.ForeColor = Color.White;
                        cmbGender.SelectedItem = Properties.Resources.Gender_Other;
                        break;
                }
            }

            // ✅ BirthDate
            if (_currentProfile.BirthDate.HasValue)
            {
                dtpBirthDate.Value = _currentProfile.BirthDate.Value;
            }

            // ✅ Address ComboBox
            if (!string.IsNullOrEmpty(_currentProfile.Address))
            {
                // Try to find exact match
                int index = cmbAddress.Items.IndexOf(_currentProfile.Address);
                if (index >= 0)
                {
                    cmbAddress.SelectedIndex = index;
                }
                else
                {
                    cmbAddress.SelectedIndex = 0; // Default
                }
            }

            // ✅ Education ComboBox
            if (!string.IsNullOrEmpty(_currentProfile.EducationLevel))
            {
                int index = cmbEducation.Items.IndexOf(_currentProfile.EducationLevel);
                if (index >= 0)
                {
                    cmbEducation.SelectedIndex = index;
                }
                else
                {
                    cmbEducation.SelectedIndex = 0;
                }
            }

            // ✅ Occupation ComboBox
            if (!string.IsNullOrEmpty(_currentProfile.Occupation))
            {
                int index = cmbOccupation.Items.IndexOf(_currentProfile.Occupation);
                if (index >= 0)
                {
                    // Found exact match in list
                    cmbOccupation.SelectedIndex = index;
                    txtOccupationOther.Visible = false; // ✅ Hide "Other" textbox
                    txtOccupationOther.Text = "";
                }
                else
                {
                    // Custom occupation - select "Other"
                    cmbOccupation.SelectedIndex = cmbOccupation.Items.Count - 1; // "Other (specify below)"
                    txtOccupationOther.Text = _currentProfile.Occupation;
                    txtOccupationOther.Visible = false; // ✅ Hide initially, will show in edit mode
                }
            }
            else
            {
                cmbOccupation.SelectedIndex = 0; // Default "Select occupation"
                txtOccupationOther.Visible = false; // ✅ Hide
                txtOccupationOther.Text = "";
            }
        }

        /// <summary>
        /// ✅ Helper to set field with gray color if empty
        /// </summary>
        private void SetFieldValue(Label label, Guna.UI2.WinForms.Guna2TextBox textBox, string value, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                label.Text = placeholder;
                label.ForeColor = Color.FromArgb(120, 120, 120); // Gray
            }
            else
            {
                label.Text = value;
                label.ForeColor = Color.White;
            }

            if (textBox != null)
            {
                textBox.Text = value ?? "";
            }
        }

        private async Task LoadAvatar()
        {
            try
            {
                if (picAvatar == null) return;

                string avatarUrl = _currentUser?.AvatarUrl;

                if (string.IsNullOrEmpty(avatarUrl))
                {
                    SetDefaultAvatar();
                    return;
                }

                if (avatarUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    avatarUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            using (var client = new System.Net.WebClient())
                            {
                                var imageBytes = await client.DownloadDataTaskAsync(avatarUrl);
                                using (var ms = new System.IO.MemoryStream(imageBytes))
                                {
                                    var image = Image.FromStream(ms);

                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        picAvatar.Image = image;
                                        picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                                    });
                                }
                            }
                        }
                        catch
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                SetDefaultAvatar();
                            });
                        }
                    });
                }
                else if (System.IO.File.Exists(avatarUrl))
                {
                    picAvatar.Image = Image.FromFile(avatarUrl);
                    picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    SetDefaultAvatar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadAvatar] Error: {ex.Message}");
                SetDefaultAvatar();
            }
        }

        private void SetDefaultAvatar()
        {
            try
            {
                const string DEFAULT_AVATAR_URL = "https://i.pinimg.com/736x/4a/4c/29/4a4c29807499a1a8085e9bde536a570a.jpg";
                picAvatar.LoadAsync(DEFAULT_AVATAR_URL);
                picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SetDefaultAvatar] Error: {ex.Message}");
                picAvatar.Image = null;
            }
        }

        private void SetReadOnlyMode()
        {
            _isEditMode = false;

            // Show value labels
            lblFullNameValue.Visible = true;
            lblEmailValue.Visible = true;
            lblPhoneValue.Visible = true;
            lblGenderValue.Visible = true;
            lblBirthDateValue.Visible = true;
            lblAddressValue.Visible = true;
            lblEducationValue.Visible = true;
            lblCompanyValue.Visible = true;
            lblOccupationValue.Visible = true;

            // Hide input controls
            txtFullName.Visible = false;
            txtEmail.Visible = false;
            txtPhone.Visible = false;
            cmbGender.Visible = false;
            dtpBirthDate.Visible = false;
            cmbAddress.Visible = false;
            cmbEducation.Visible = false;
            txtCompany.Visible = false;
            cmbOccupation.Visible = false;
            txtOccupationOther.Visible = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;

            // Show banner if exists
            if (panelBanner != null && _currentProfile != null)
            {
                CheckProfileCompleteness();
            }

            ClearErrors();
        }

        private void SetEditMode()
        {
            _isEditMode = true;

            lblFullNameValue.Visible = false;
            lblEmailValue.Visible = false;
            lblPhoneValue.Visible = false;
            lblGenderValue.Visible = false;
            lblBirthDateValue.Visible = false;
            lblAddressValue.Visible = false;
            lblEducationValue.Visible = false;
            lblCompanyValue.Visible = false;
            lblOccupationValue.Visible = false;

            txtFullName.Visible = true;
            txtEmail.Visible = true;
            txtPhone.Visible = true;
            cmbGender.Visible = true;
            dtpBirthDate.Visible = true;
            cmbAddress.Visible = true;
            cmbEducation.Visible = true;
            txtCompany.Visible = true;
            cmbOccupation.Visible = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;

            // ✅ txtOccupationOther chỉ hiện khi chọn "Other"
            // Kiểm tra current selection
            if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1)
            {
                txtOccupationOther.Visible = true;
            }
            else
            {
                txtOccupationOther.Visible = false;
            }

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;

            // Hide banner during edit
            if (panelBanner != null)
            {
                panelBanner.Visible = false;
            }
        }

        private void ClearErrors()
        {
            lblFullNameError.Visible = false;
            lblPhoneError.Visible = false;

            Color defaultBorder = Color.FromArgb(213, 218, 223);

            txtFullName.BorderColor = defaultBorder;
            txtPhone.BorderColor = defaultBorder;
            cmbGender.BorderColor = defaultBorder;
            dtpBirthDate.BorderColor = defaultBorder;
            cmbAddress.BorderColor = defaultBorder;
            cmbEducation.BorderColor = defaultBorder;
            txtCompany.BorderColor = defaultBorder;
            cmbOccupation.BorderColor = defaultBorder;
            txtOccupationOther.BorderColor = defaultBorder;
        }

        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();

            bool hasErrors = false;

            // Validate FullName
            string fullName = txtFullName.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(fullName))
            {
                lblFullNameError.Text = "Full name is required";
                lblFullNameError.Visible = true;
                txtFullName.BorderColor = Color.OrangeRed;
                errors.Add(lblFullNameError.Text);
                hasErrors = true;
            }
            else if (fullName.Length < 2 || fullName.Length > 100)
            {
                lblFullNameError.Text = "Full name must be between 2 and 100 characters";
                lblFullNameError.Visible = true;
                txtFullName.BorderColor = Color.OrangeRed;
                errors.Add(lblFullNameError.Text);
                hasErrors = true;
            }

            // Validate Phone (optional)
            string phoneText = txtPhone.Text?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(phoneText))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneText, @"^\d+$"))
                {
                    lblPhoneError.Text = "Phone must contain only digits";
                    lblPhoneError.Visible = true;
                    txtPhone.BorderColor = Color.OrangeRed;
                    errors.Add(lblPhoneError.Text);
                    hasErrors = true;
                }
                else if (phoneText.Length != 10)
                {
                    lblPhoneError.Text = "Phone must be exactly 10 digits";
                    lblPhoneError.Visible = true;
                    txtPhone.BorderColor = Color.OrangeRed;
                    errors.Add(lblPhoneError.Text);
                    hasErrors = true;
                }
                else if (!phoneText.StartsWith("0"))
                {
                    lblPhoneError.Text = "Phone must start with 0";
                    lblPhoneError.Visible = true;
                    txtPhone.BorderColor = Color.OrangeRed;
                    errors.Add(lblPhoneError.Text);
                    hasErrors = true;
                }
            }

            return !hasErrors;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SetEditMode();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            PopulateFormFields();
            SetReadOnlyMode();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out var validationErrors))
            {
                return;
            }

            btnSave.Enabled = false;
            btnSave.Text = Properties.Resources.ProfileForm_Saving;

            try
            {
                // ✅ Get Gender value
                string genderValue = null;
                if (cmbGender.SelectedItem != null)
                {
                    string selectedGender = cmbGender.SelectedItem.ToString();
                    if (selectedGender == Properties.Resources.Gender_Male)
                        genderValue = "Male";
                    else if (selectedGender == Properties.Resources.Gender_Female)
                        genderValue = "Female";
                    else if (selectedGender == Properties.Resources.Gender_Other)
                        genderValue = "Other";
                }

                // ✅ Get Address value
                string addressValue = "";
                if (cmbAddress.SelectedIndex > 0)
                {
                    addressValue = cmbAddress.SelectedItem.ToString();
                }

                // ✅ Get Education value
                string educationValue = "";
                if (cmbEducation.SelectedIndex > 0)
                {
                    educationValue = cmbEducation.SelectedItem.ToString();
                }

                // ✅ Get Occupation value
                string occupationValue = "";
                if (cmbOccupation.SelectedIndex > 0)
                {
                    if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1) // "Other"
                    {
                        occupationValue = txtOccupationOther.Text?.Trim() ?? "";
                    }
                    else
                    {
                        occupationValue = cmbOccupation.SelectedItem.ToString();
                    }
                }

                var updateDto = new UpdateProfileDto
                {
                    FullName = txtFullName.Text.Trim(),
                    PhoneNumber = txtPhone.Text?.Trim() ?? "",
                    Gender = genderValue,
                    BirthDate = dtpBirthDate.Value,
                    Address = addressValue,
                    EducationLevel = educationValue,
                    Company = txtCompany.Text?.Trim() ?? "",
                    Occupation = occupationValue,
                    AvatarUrl = _currentUser?.AvatarUrl ?? ""
                };

                var response = await _profileService.UpdateProfileAsync(_userId, updateDto);

                if (response?.Success == true)
                {
                    _currentProfile = response.Data?.Profile;
                    _currentUser = response.Data?.User;

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_ProfileUpdateSuccess,
                        Properties.Resources.Title_Success
                    );

                    PopulateFormFields();
                    SetReadOnlyMode();
                    await LoadAvatar();

                    // Re-check completeness
                    CheckProfileCompleteness();
                }
                else
                {
                    CustomMessageBox.ShowError(
                        response?.Message ?? Properties.Resources.Message_ProfileUpdateFailed,
                        Properties.Resources.Title_Error
                    );
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}", Properties.Resources.Title_Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = Properties.Resources.ProfileForm_BtnSave;
            }
        }

        /// <summary>
        /// ✅ Check Google account and show appropriate dialog
        /// </summary>
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var authProp = _currentUser?.GetType().GetProperty("AuthProvider");
            string authProvider = authProp?.GetValue(_currentUser) as string;

            if (!string.IsNullOrWhiteSpace(authProvider) &&
                authProvider.Equals("Google", StringComparison.OrdinalIgnoreCase))
            {
                var result = MessageBox.Show(
                    "🔐 Google Account\n\n" +
                    "You signed in with Google. Your password is managed by Google.\n\n" +
                    "Would you like to open Google Account settings?",
                    "Password Management",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://myaccount.google.com/security");
                }
                return;
            }

            var changePasswordForm = new ChangePasswordForm(_userId);
            changePasswordForm.Owner = this;
            changePasswordForm.ShowDialog();
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
        // ✅ THÊM LOGOUT HANDLER
        private async void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                var result = CustomMessageBox.ShowQuestion(
                    Properties.Resources.Message_LogoutConfirm,
                    Properties.Resources.Title_Confirmation
                );

                if (result != DialogResult.Yes)
                {
                    return;
                }

                btnLogout.Enabled = false;
                Cursor = Cursors.WaitCursor;

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[ProfileForm] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[ProfileForm] ✅ Logout successful");

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    var loginForm = new LoginForm();

                    // ✅ Đóng tất cả form liên quan
                    if (_homeForm != null && !_homeForm.IsDisposed)
                    {
                        _homeForm.Close();
                    }

                    if (_parentForm != null && !_parentForm.IsDisposed)
                    {
                        _parentForm.Close();
                    }

                    this.Hide();
                    loginForm.Show();
                    this.Dispose();

                    Console.WriteLine("[ProfileForm] ✅ Returned to LoginForm");
                    Console.WriteLine(new string('=', 60) + "\n");
                }
                else
                {
                    Console.WriteLine($"[ProfileForm] ❌ Logout failed: {response?.Message}");

                    CustomMessageBox.ShowError(
                        response?.Message ?? Properties.Resources.Message_LogoutFailed,
                        Properties.Resources.Title_Error
                    );

                    btnLogout.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileForm] ❌ Exception: {ex.Message}");

                CustomMessageBox.ShowError(
                    $"{Properties.Resources.Message_LogoutError}: {ex.Message}",
                    Properties.Resources.Title_ConnectionError
                );

                btnLogout.Enabled = true;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }


        // ✅ Banner Events
        private void btnBannerAdd_Click(object sender, EventArgs e)
        {
            SetEditMode();
            if (panelBanner != null)
            {
                panelBanner.Visible = false;
            }
        }

        private void btnBannerClose_Click(object sender, EventArgs e)
        {
            if (panelBanner != null)
            {
                panelBanner.Visible = false;
            }
        }

        // ✅ ComboBox Events
        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e) => ClearErrors();

        private void cmbAddress_SelectedIndexChanged(object sender, EventArgs e) => ClearErrors();

        private void cmbEducation_SelectedIndexChanged(object sender, EventArgs e) => ClearErrors();

        private void cmbOccupation_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ✅ Chỉ show txtOccupationOther khi đang ở Edit mode VÀ chọn "Other"
            if (_isEditMode && cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1)
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

        // ✅ TextBox Events
        private void txtFullName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                lblFullNameError.Visible = false;
                txtFullName.BorderColor = Color.FromArgb(213, 218, 223);
            }
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            string phoneText = txtPhone.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(phoneText))
            {
                lblPhoneError.Visible = false;
                txtPhone.BorderColor = Color.FromArgb(213, 218, 223);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneText, @"^\d+$"))
            {
                lblPhoneError.Text = "Only digits allowed";
                lblPhoneError.Visible = true;
                txtPhone.BorderColor = Color.Orange;
            }
            else if (phoneText.Length > 10)
            {
                lblPhoneError.Text = "Maximum 10 digits";
                lblPhoneError.Visible = true;
                txtPhone.BorderColor = Color.Orange;
            }
            else if (phoneText.Length < 10 && phoneText.Length > 0)
            {
                lblPhoneError.Text = $"{10 - phoneText.Length} more digits needed";
                lblPhoneError.Visible = true;
                txtPhone.BorderColor = Color.Orange;
            }
            else if (!phoneText.StartsWith("0"))
            {
                lblPhoneError.Text = "Must start with 0";
                lblPhoneError.Visible = true;
                txtPhone.BorderColor = Color.Orange;
            }
            else
            {
                lblPhoneError.Visible = false;
                txtPhone.BorderColor = Color.FromArgb(94, 148, 255);
            }
        }

        private void dtpBirthDate_ValueChanged(object sender, EventArgs e) => ClearErrors();
        private void txtCompany_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtOccupationOther_TextChanged(object sender, EventArgs e) => ClearErrors();
    }
}