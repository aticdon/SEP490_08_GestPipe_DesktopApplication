using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

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
                ListRequestGestureForm requestForm = new ListRequestGestureForm(_homeForm);
                requestForm.Show();
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
                CheckProfileCompleteness();
                await _apiClient.SetUserLanguageAsync(_userId, cultureCode);
            }
            catch (Exception ex)
            {
                //CustomMessageBox.ShowError(
                //    Properties.Resources.Message_ChangeLanguageFailed,
                //    Properties.Resources.Title_Error
                //);
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
                btnBannerAdd.Text = Properties.Resources.ProfileForm_BtnBannerAdd;

                // Gender combo
                cmbGender.Items.Clear();
                cmbGender.Items.AddRange(new object[] {
                    Properties.Resources.Gender_Male,
                    Properties.Resources.Gender_Female,
                    Properties.Resources.Gender_Other
                });
                RebindLocalizedCombos();
                if (_currentProfile != null && _currentUser != null)
                {
                    PopulateFormFields();
                }
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
                //lblBannerMessage.Text = $"Your profile is {percentage}% complete. Add more info for a better experience!";
                string bannerEN = $"Your profile is {percentage}% complete. Add more info for a better experience!";
                string bannerVN = $"Hồ sơ của bạn đã hoàn thành {percentage}% hãy thêm thông tin để trải nghiệm tốt hơn";
                lblBannerMessage.Text = I18nHelper.GetString(bannerEN, bannerVN);
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
            btnChangeAvatar.Text = _currentProfile.FullName ?? "User";
            txtFullName.Text = _currentProfile.FullName ?? "";

            // Email
            lblEmailValue.Text = _currentUser.Email ?? "-";
            txtEmail.Text = _currentUser.Email ?? "";

            string notAdded = I18nHelper.GetString("Not added", "Chưa thêm");
            string notSpecified = I18nHelper.GetString("Not specified", "Chưa xác định");
            string notSet = I18nHelper.GetString("Not set", "Chưa đặt");
            string notProvided = I18nHelper.GetString("Not provided", "Chưa cung cấp");
            // ✅ Optional fields with gray color
            SetFieldValue(lblPhoneValue, txtPhone, _currentProfile.PhoneNumber, notAdded);
            //SetFieldValue(lblGenderValue, null, _currentProfile.Gender, notSpecified);
            if (string.IsNullOrWhiteSpace(_currentProfile.Gender))
            {
                lblGenderValue.Text = notSpecified;
                lblGenderValue.ForeColor = Color.FromArgb(120, 120, 120);
            }
            else
            {
                lblGenderValue.Text = GetGenderDisplay(_currentProfile.Gender);
                lblGenderValue.ForeColor = Color.White;

                // Đồng bộ combobox theo key trong DB
                switch (_currentProfile.Gender.ToLower())
                {
                    case "male":
                        cmbGender.SelectedItem = Properties.Resources.Gender_Male;
                        break;
                    case "female":
                        cmbGender.SelectedItem = Properties.Resources.Gender_Female;
                        break;
                    case "other":
                        cmbGender.SelectedItem = Properties.Resources.Gender_Other;
                        break;
                }
            }
            //SetFieldValue(lblBirthDateValue, null, _currentProfile.BirthDate?.ToString("MMMM d, yyyy"), notSet);
            string birthDateDisplay = null;
            if (_currentProfile.BirthDate.HasValue)
            {
                var date = _currentProfile.BirthDate.Value;

                if (_currentCultureCode == "vi-VN")
                {
                    // Bạn có thể đổi "dd/MM/yyyy" nếu thích dạng số
                    birthDateDisplay = date.ToString("dd MMMM, yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                    // ví dụ: "06 Tháng một, 2025"
                }
                else
                {
                    birthDateDisplay = date.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"));
                    // ví dụ: "January 6, 2025"
                }
            }

            SetFieldValue(lblBirthDateValue, null, birthDateDisplay, notSet);

            SetFieldValue(lblCompanyValue, txtCompany, _currentProfile.Company, notSpecified);
            // ✅ Address
            if (string.IsNullOrWhiteSpace(_currentProfile.Address))
            {
                lblAddressValue.Text = notProvided;
                lblAddressValue.ForeColor = Color.FromArgb(120, 120, 120);
            }
            else
            {
                lblAddressValue.Text = GetDisplayText(_addressOptions, _currentProfile.Address);
                lblAddressValue.ForeColor = Color.White;
            }

            // ✅ Education
            if (string.IsNullOrWhiteSpace(_currentProfile.EducationLevel))
            {
                lblEducationValue.Text = notAdded;
                lblEducationValue.ForeColor = Color.FromArgb(120, 120, 120);
            }
            else
            {
                lblEducationValue.Text = GetDisplayText(_educationOptions, _currentProfile.EducationLevel);
                lblEducationValue.ForeColor = Color.White;
            }

            // ✅ Occupation: nếu là option có sẵn thì dùng display, nếu là custom text thì giữ nguyên
            if (string.IsNullOrWhiteSpace(_currentProfile.Occupation))
            {
                lblOccupationValue.Text = notAdded;
                lblOccupationValue.ForeColor = Color.FromArgb(120, 120, 120);
            }
            else
            {
                var optOcc = _occupationOptions.FirstOrDefault(o => o.Key == _currentProfile.Occupation);
                if (optOcc != null)
                {
                    lblOccupationValue.Text = GetDisplayText(optOcc);
                }
                else
                {
                    // custom occupation (tự gõ khi chọn Other)
                    lblOccupationValue.Text = _currentProfile.Occupation;
                }
                lblOccupationValue.ForeColor = Color.White;
            }

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
            RebindLocalizedCombos();
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
            btnChangeAvatar.Visible = true;
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

            btnChangeAvatar.Visible = false;
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
                //string genderValue = null;
                Console.WriteLine($"[DEBUG] Gender SelectedIndex = {cmbGender.SelectedIndex}");
                Console.WriteLine($"[DEBUG] Gender SelectedItem = {cmbGender.SelectedItem}");
                //if (cmbGender.SelectedItem != null)
                //{
                //    string selectedGender = cmbGender.SelectedItem.ToString();
                //    if (selectedGender == Properties.Resources.Gender_Male)
                //        genderValue = "Male";
                //    else if (selectedGender == Properties.Resources.Gender_Female)
                //        genderValue = "Female";
                //    else if (selectedGender == Properties.Resources.Gender_Other)
                //        genderValue = "Other";
                //}
                string genderValue = null;
                if (cmbGender.SelectedIndex >= 0)
                {
                    if (cmbGender.SelectedIndex == 0) genderValue = "Male";
                    else if (cmbGender.SelectedIndex == 1) genderValue = "Female";
                    else if (cmbGender.SelectedIndex == 2) genderValue = "Other";
                }
                Console.WriteLine($"[DEBUG] genderValue before send = {genderValue}");
                // ✅ Get Address value
                //string addressValue = "";
                //if (cmbAddress.SelectedIndex > 0)
                //{
                //    addressValue = cmbAddress.SelectedItem.ToString();
                //}

                //// ✅ Get Education value
                //string educationValue = "";
                //if (cmbEducation.SelectedIndex > 0)
                //{
                //    educationValue = cmbEducation.SelectedItem.ToString();
                //}

                //// ✅ Get Occupation value
                //string occupationValue = "";
                //if (cmbOccupation.SelectedIndex > 0)
                //{
                //    if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1) // "Other"
                //    {
                //        occupationValue = txtOccupationOther.Text?.Trim() ?? "";
                //    }
                //    else
                //    {
                //        occupationValue = cmbOccupation.SelectedItem.ToString();
                //    }
                //}
                // ✅ Address: lưu Key vào DB
                string addressValue = "";
                if (cmbAddress.SelectedIndex > 0)
                {
                    var idx = cmbAddress.SelectedIndex - 1; // trừ 1 vì có item placeholder ở đầu
                    if (idx >= 0 && idx < _addressOptions.Count)
                    {
                        addressValue = _addressOptions[idx].Key;
                    }
                }

                // ✅ Education: lưu Key
                string educationValue = "";
                if (cmbEducation.SelectedIndex > 0)
                {
                    var idx = cmbEducation.SelectedIndex - 1;
                    if (idx >= 0 && idx < _educationOptions.Count)
                    {
                        educationValue = _educationOptions[idx].Key;
                    }
                }

                // ✅ Occupation: lưu Key nếu là option có sẵn, còn "Other" thì lưu text tự do
                string occupationValue = "";
                if (cmbOccupation.SelectedIndex > 0)
                {
                    // Nếu chọn dòng cuối cùng = "Other (specify below)" → lưu text người dùng nhập
                    if (cmbOccupation.SelectedIndex == cmbOccupation.Items.Count - 1)
                    {
                        occupationValue = txtOccupationOther.Text?.Trim() ?? "";
                    }
                    else
                    {
                        var idx = cmbOccupation.SelectedIndex - 1;
                        if (idx >= 0 && idx < _occupationOptions.Count)
                        {
                            occupationValue = _occupationOptions[idx].Key;
                        }
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

       
        private async void btnChangeAvatar_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            dlg.Title = "Chọn ảnh đại diện mới";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filePath = dlg.FileName;

                try
                {
                    // 1. Hiển thị lên UI trước
                    picAvatar.Image = Image.FromFile(filePath);

                    // 2. Upload ảnh lên server và nhận về URL
                    Console.WriteLine("[ProfileForm] Uploading avatar...");
                    string avatarUrl = await _profileService.UploadAvatarAsync(filePath);

                    if (!string.IsNullOrWhiteSpace(avatarUrl))
                    {
                        Console.WriteLine($"[ProfileForm] Upload success, URL: {avatarUrl}");

                        // 3. ✅ CHỈ CẬP NHẬT AVATAR (DÙNG ENDPOINT PATCH)
                        Console.WriteLine("[ProfileForm] Updating avatar only...");
                        var response = await _profileService.UpdateAvatarOnlyAsync(_userId, avatarUrl);

                        if (response?.Success == true)
                        {
                            Console.WriteLine("[ProfileForm] ✅ Avatar updated successfully in database!");
                            _currentUser.AvatarUrl = avatarUrl; // Cập nhật local
                            CustomMessageBox.ShowSuccess(I18nHelper.GetString("Profile photo updated successfully!", "Cập nhật ảnh đại diện thành công!"), I18nHelper.GetString("Success","Thành công"));
                            await LoadAvatar(); // Load lại avatar từ URL mới
                        }
                        else
                        {
                            Console.WriteLine($"[ProfileForm] ❌ Failed to update avatar: {response?.Message}");
                            CustomMessageBox.ShowError(I18nHelper.GetString("Profile picture update failed!", "Cập nhật ảnh đại diện thất bại!"), I18nHelper.GetString("Fail", "Lỗi"));
                        }
                    }
                    else
                    {
                        Console.WriteLine("[ProfileForm] ❌ Upload failed, no URL returned");
                        CustomMessageBox.ShowError(I18nHelper.GetString("Cannot upload avatar!", "Không upload được ảnh đại diện!"), I18nHelper.GetString("Fail", "Lỗi"));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ProfileForm] ❌ Exception: {ex.Message}");
                    //CustomMessageBox.ShowError($"Lỗi: {ex.Message}", "Lỗi");
                }
            }
        }

        private void dtpBirthDate_ValueChanged(object sender, EventArgs e) => ClearErrors();
        private void txtCompany_TextChanged(object sender, EventArgs e) => ClearErrors();
        private void txtOccupationOther_TextChanged(object sender, EventArgs e) => ClearErrors();

        private void btnInstruction_Click(object sender, EventArgs e)
        {
            InstructionForm instructionForm = new InstructionForm(_homeForm);
            instructionForm.Show();
            this.Hide();
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }

        private readonly List<LocalizedOption> _addressOptions = new()
{
    new LocalizedOption("Thành phố Hà Nội", "Ha Noi City", "Thành phố Hà Nội"),
    new LocalizedOption("Thành phố Huế", "Hue City", "Thành phố Huế"),
    new LocalizedOption("Tỉnh Lai Châu", "Lai Chau Province", "Tỉnh Lai Châu"),
    new LocalizedOption("Tỉnh Điện Biên", "Dien Bien Province", "Tỉnh Điện Biên"),
    new LocalizedOption("Tỉnh Sơn La", "Son La Province", "Tỉnh Sơn La"),
    new LocalizedOption("Tỉnh Lạng Sơn", "Lang Son Province", "Tỉnh Lạng Sơn"),
    new LocalizedOption("Tỉnh Quảng Ninh", "Quang Ninh Province", "Tỉnh Quảng Ninh"),
    new LocalizedOption("Tỉnh Thanh Hóa", "Thanh Hoa Province", "Tỉnh Thanh Hóa"),
    new LocalizedOption("Tỉnh Nghệ An", "Nghe An Province", "Tỉnh Nghệ An"),
    new LocalizedOption("Tỉnh Hà Tĩnh", "Ha Tinh Province", "Tỉnh Hà Tĩnh"),
    new LocalizedOption("Tỉnh Cao Bàng", "Cao Bang Province", "Tỉnh Cao Bằng"),
    new LocalizedOption("Tỉnh Tuyên Quang", "Tuyen Quang Province", "Tỉnh Tuyên Quang"),
    new LocalizedOption("Tỉnh Lào Cai", "Lao Cai Province", "Tỉnh Lào Cai"),
    new LocalizedOption("Tỉnh Thái Nguyên", "Thai Nguyen Province", "Tỉnh Thái Nguyên"),
    new LocalizedOption("Tỉnh Phú Thọ", "Phu Tho Province", "Tỉnh Phú Thọ"),
    new LocalizedOption("Tỉnh Bắc Ninh", "Bac Ninh Province", "Tỉnh Bắc Ninh"),
    new LocalizedOption("Tỉnh Hưng Yên", "Hung Yen Province", "Tỉnh Hưng Yên"),
    new LocalizedOption("Thành Phố Hải Phòng", "Hai Phong City", "Thành Phố Hải Phòng"),
    new LocalizedOption("Tỉnh Ninh Bình", "Ninh Binh Province", "Tỉnh Ninh Bình"),
    new LocalizedOption("Tỉnh Quảng Trị", "Quang Tri Province", "Tỉnh Quảng Trị"),
    new LocalizedOption("Thành Phố Đà Nẵng", "Da Nang City", "Thành Phố Đà Nẵng"),
    new LocalizedOption("Tỉnh Bình Định", "Binh Dinh Province", "Tỉnh Bình Định"),
    new LocalizedOption("Tỉnh Khánh Hòa", "Khanh Hoa Province", "Tỉnh Khanh Hoa"),
    new LocalizedOption("Tỉnh Đắk Lắk", "Dien Bien Province", "Tỉnh Đắk Lắk"),
    new LocalizedOption("Tỉnh Gia Lai", "Gia LaiProvince", "Tỉnh Gia Lai"),
    new LocalizedOption("Tỉnh Lâm Đồng", "Lam Dong Province", "Tỉnh Lâm Đồng"),
    new LocalizedOption("Tỉnh Đồng Nai", "Dong Nai Province", "Tỉnh Đồng Nai"),
    new LocalizedOption("Thành phố Hồ Chí Minh", "Ho Chi Minh City", "Thành phố Hồ Chí Minh"),
    new LocalizedOption("Tỉnh Tây Ninh", "Thanh Hoa Province", "Tỉnh Thanh Hóa"),
    new LocalizedOption("Tỉnh Tiền Giang", "Tien Giang Province", "Tỉnh Tien Giang"),
    new LocalizedOption("Tỉnh Vĩnh Long", "Vinh Long Province", "Tỉnh Vĩnh Long"),
    new LocalizedOption("Thành phố Cần Thơ", "Can Tho City", "Thành phố Cần Thơ"),
    new LocalizedOption("Tỉnh An Giang", "An Giang Province", "Tỉnh An Giang"),
    new LocalizedOption("Tỉnh Cà Mau", "Ca Mau Province", "Tỉnh Cà Mau")
};
        private readonly List<LocalizedOption> _educationOptions = new()
{
    // Key = chuỗi bạn muốn lưu trong DB (có thể giữ luôn EN cũ)
    new LocalizedOption("High School", "High school", "Trung học phổ thông"),
    new LocalizedOption("College", "College", "Cao đẳng"),
    new LocalizedOption("Bachelor", "Bachelor", "Đại học"),
    new LocalizedOption("Master", "Master", "Thạc sĩ"),
    new LocalizedOption("PhD", "PhD", "Tiến sĩ")
};
        private readonly List<LocalizedOption> _occupationOptions = new()
{
    new LocalizedOption("IT/Software/Technology",     "IT",    "CNTT"),
    new LocalizedOption("Teacher/Education",          "Teacher",           "Giáo viên"),
    new LocalizedOption("Marketing/PR/Advertising",   "Marketing",  "Quảng cáo"),
    new LocalizedOption("Sales/Business/Trade",       "Business",      "Bán hàng"),
    new LocalizedOption("Finance/Banking",            "Finance",             "Tài chính"),
    new LocalizedOption("Designer/Creative",          "Designer",           "Thiết kế"),
    new LocalizedOption("Healthcare/Medical",         "Healthcare",          "Y tế"),
    new LocalizedOption("Engineering/Technical",      "Engineering",       "Kỹ sư"),
    new LocalizedOption("Construction/Architecture",  "Construction",   "Xây dựng"),
    new LocalizedOption("Legal/Law",                  "Law",                   "Luật"),
    new LocalizedOption("Hospitality/Tourism",        "Tourism",         "Du lịch"),
    new LocalizedOption("Science/Research",           "Science",            "Khoa học"),
    new LocalizedOption("Administrative/Clerical",    "Administrative",     "Hành chính"),
    new LocalizedOption("Student",                    "Student",                       "Sinh viên"),
    new LocalizedOption("Other",                      "Other (specify below)",         "Khác (ghi bên dưới)")
};
        private string GetDisplayText(LocalizedOption opt)
        {
            if (opt == null) return string.Empty;

            // vi-VN: tiếng Việt trước
            if (_currentCultureCode == "vi-VN")
                return $"{opt.TextVi} ({opt.TextEn})";

            // en-US: tiếng Anh trước
            return $"{opt.TextEn} ({opt.TextVi})";
        }

        private string GetGenderDisplay(string genderKey)
        {
            if (string.IsNullOrWhiteSpace(genderKey))
                return I18nHelper.GetString("Not specified", "Chưa xác định");

            genderKey = genderKey.ToLowerInvariant();

            string en = genderKey switch
            {
                "male" => "Male",
                "female" => "Female",
                "other" => "Other",
                _ => genderKey
            };

            string vi = genderKey switch
            {
                "male" => "Nam",
                "female" => "Nữ",
                "other" => "Khác",
                _ => genderKey
            };

            // vi-VN: tiếng Việt trước, en-US: tiếng Anh trước
            return _currentCultureCode == "vi-VN"
                ? $"{vi} ({en})"
                : $"{en} ({vi})";
        }

        // Lấy display text theo key
        private string GetDisplayText(List<LocalizedOption> list, string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            var opt = list.FirstOrDefault(o => o.Key == key);
            return opt != null ? GetDisplayText(opt) : key; // fallback hiển thị key nếu không map được
        }
        private void BindAddressCombo()
        {
            cmbAddress.Items.Clear();

            // placeholder song ngữ
            cmbAddress.Items.Add(
                I18nHelper.GetString("Select Province/City", "Chọn Tỉnh/Thành phố"));

            foreach (var opt in _addressOptions)
            {
                cmbAddress.Items.Add(GetDisplayText(opt));
            }

            // set SelectedIndex theo _currentProfile.Address (key)
            if (_currentProfile != null && !string.IsNullOrWhiteSpace(_currentProfile.Address))
            {
                int idx = _addressOptions.FindIndex(o => o.Key == _currentProfile.Address);
                cmbAddress.SelectedIndex = idx >= 0 ? idx + 1 : 0;
            }
            else
            {
                cmbAddress.SelectedIndex = 0;
            }
        }
        private void BindEducationCombo()
        {
            cmbEducation.Items.Clear();

            cmbEducation.Items.Add(
                I18nHelper.GetString("Select educational level", "Chọn trình độ học vấn"));

            foreach (var opt in _educationOptions)
            {
                cmbEducation.Items.Add(GetDisplayText(opt));
            }

            if (_currentProfile != null && !string.IsNullOrWhiteSpace(_currentProfile.EducationLevel))
            {
                int idx = _educationOptions.FindIndex(o => o.Key == _currentProfile.EducationLevel);
                cmbEducation.SelectedIndex = idx >= 0 ? idx + 1 : 0;
            }
            else
            {
                cmbEducation.SelectedIndex = 0;
            }
        }
        private void BindOccupationCombo()
        {
            cmbOccupation.Items.Clear();

            cmbOccupation.Items.Add(
                I18nHelper.GetString("Select occupation", "Chọn nghề nghiệp"));

            foreach (var opt in _occupationOptions)
            {
                cmbOccupation.Items.Add(GetDisplayText(opt));
            }

            if (_currentProfile != null && !string.IsNullOrWhiteSpace(_currentProfile.Occupation))
            {
                int idx = _occupationOptions.FindIndex(o => o.Key == _currentProfile.Occupation);
                if (idx >= 0)
                {
                    // Occupation nằm trong list chuẩn
                    cmbOccupation.SelectedIndex = idx + 1;
                    txtOccupationOther.Visible = false;
                }
                else
                {
                    // Custom text → chọn luôn "Other"
                    cmbOccupation.SelectedIndex = cmbOccupation.Items.Count - 1;

                    if (_isEditMode)
                    {
                        txtOccupationOther.Visible = true;
                        txtOccupationOther.Text = _currentProfile.Occupation;
                    }
                    else
                    {
                        txtOccupationOther.Visible = false;
                    }
                }
            }
            else
            {
                cmbOccupation.SelectedIndex = 0;
                txtOccupationOther.Visible = false;
            }
        }

        private void RebindLocalizedCombos()
        {
            BindAddressCombo();
            BindEducationCombo();
            BindOccupationCombo();
        }
    }
}