using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views.Auth;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class HomeUser : Form
    {
        private const int BUTTON_SPACING = 40;
        private int buttonBottomMargin;
        private readonly ApiClient _apiClient;
        private readonly AuthService _authService;
        private readonly string _currentUserId;
        private string _currentCultureCode = "en-US";

        public HomeUser(string currentUserId)
        {
            InitializeComponent();

            buttonBottomMargin = pnlMain.Height - btnTraining.Bottom;
            this.pnlMain.Resize += new System.EventHandler(this.guna2Panel2_Resize);
            CenterButtons();

            // Event handlers
            btnPresent.Click += BtnPresent_Click;
            btnTraining.Click += BtnTraining_Click;
            btnGestureControl.Click += BtnGestureControl_Click;

            _currentUserId = currentUserId;
            _apiClient = new ApiClient("https://localhost:7219");
            _authService = new AuthService();

            this.Load += HomeUser_Load;
            btnLanguageEN.Click += async (s, e) => await ChangeLanguageAsync("en-US");
            btnLanguageVN.Click += async (s, e) => await ChangeLanguageAsync("vi-VN");

            CultureManager.CultureChanged += (s, e) =>
            {
                ResourceHelper.SetCulture(CultureManager.CurrentCultureCode, this);
                ApplyResourceToControls();
            };
        }

        private async void HomeUser_Load(object sender, EventArgs e)
        {
            await LoadUserAndApplyLanguageAsync();
        }

        private void CenterButtons()
        {
            int panelWidth = pnlMain.ClientSize.Width;
            int panelHeight = pnlMain.ClientSize.Height;
            int totalWidth = btnTraining.Width + btnPresent.Width + BUTTON_SPACING;
            int startX = (panelWidth - totalWidth) / 2;
            int buttonY = panelHeight - btnTraining.Height - buttonBottomMargin;

            btnTraining.Location = new Point(startX, buttonY);
            btnPresent.Location = new Point(startX + btnTraining.Width + BUTTON_SPACING, buttonY);
        }

        private void guna2Panel2_Resize(object sender, EventArgs e)
        {
            CenterButtons();
        }

        private void BtnPresent_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(this);
            form1.Show();
            this.Hide();
        }

        private void BtnTraining_Click(object sender, EventArgs e)
        {
            FormCustomGesture customForm = new FormCustomGesture(this);
            customForm.Show();
            this.Hide();
            MessageBox.Show("Training feature coming soon!");
        }

        private void BtnGestureControl_Click(object sender, EventArgs e)
        {
            FormDefaultGesture defaultGesture = new FormDefaultGesture(this);
            defaultGesture.Show();
            this.Hide();
        }

        private async Task LoadUserAndApplyLanguageAsync()
        {
            try
            {
                var user = await _apiClient.GetUserAsync(_currentUserId);
                _currentCultureCode = (user != null && !string.IsNullOrWhiteSpace(user.Language)) ? user.Language : "en-US";
                CultureManager.CurrentCultureCode = _currentCultureCode;
                ResourceHelper.SetCulture(_currentCultureCode, this);
                ApplyResourceToControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể load user: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private async Task ChangeLanguageAsync(string cultureCode)
        {
            try
            {
                await _apiClient.SetUserLanguageAsync(_currentUserId, cultureCode);
                _currentCultureCode = cultureCode;
                CultureManager.CurrentCultureCode = _currentCultureCode;
                ResourceHelper.SetCulture(_currentCultureCode, this);
                ApplyResourceToControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể đổi ngôn ngữ: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void ApplyResourceToControls()
        {
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnVersion.Text = Properties.Resources.Btn_Version;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnPresent.Text = Properties.Resources.Btn_Present;
            btnCustomeGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnTraining.Text = Properties.Resources.Btn_Training;
            lblWelcome.Text = Properties.Resources.Home_Welcome;
            lblSubtitle.Text = Properties.Resources.LblSubtitle;
            btnPresentation.Text = Properties.Resources.Btn_Present;
        }

        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            FormUserGesture usergestureForm = new FormUserGesture(this);
            usergestureForm.Show();
            this.Hide();
        }

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(this);
            form1.Show();
            this.Hide();
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }

        // ✅ LOGOUT BUTTON IMPLEMENTATION
        private async void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Hiển thị confirm dialog
                var confirmMessage = (_currentCultureCode == "vi-VN")
                    ? "Bạn có chắc chắn muốn đăng xuất?"
                    : "Are you sure you want to logout?";

                var confirmTitle = (_currentCultureCode == "vi-VN")
                    ? "Xác nhận"
                    : "Confirmation";

                var result = MessageBox.Show(
                    confirmMessage,
                    confirmTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result != DialogResult.Yes)
                {
                    return;
                }

                // ✅ Disable button để tránh click nhiều lần
                btnLogout.Enabled = false;

                // ✅ Gọi API logout
                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    // ✅ Hiển thị thông báo thành công
                    var successMessage = (_currentCultureCode == "vi-VN")
                        ? "Đăng xuất thành công!"
                        : "Logged out successfully!";

                    var successTitle = (_currentCultureCode == "vi-VN")
                        ? "Thành công"
                        : "Success";

                    MessageBox.Show(
                        successMessage,
                        successTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // ✅ Đóng tất cả forms và mở LoginForm
                    this.Hide();

                    var loginForm = new LoginForm();
                    loginForm.FormClosed += (s, args) => Application.Exit();
                    loginForm.Show();

                    // ✅ Dispose HomeUser form
                    this.Dispose();
                }
                else
                {
                    // ✅ Hiển thị lỗi từ backend
                    var errorMessage = response?.Message ??
                        ((_currentCultureCode == "vi-VN") ? "Đăng xuất thất bại." : "Failed to logout.");

                    var errorTitle = (_currentCultureCode == "vi-VN") ? "Lỗi" : "Error";

                    MessageBox.Show(
                        errorMessage,
                        errorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    btnLogout.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                // ✅ Hiển thị lỗi kết nối
                var errorMessage = (_currentCultureCode == "vi-VN")
                    ? $"Lỗi khi đăng xuất: {ex.Message}"
                    : $"Error during logout: {ex.Message}";

                var errorTitle = (_currentCultureCode == "vi-VN")
                    ? "Lỗi Kết Nối"
                    : "Connection Error";

                MessageBox.Show(
                    errorMessage,
                    errorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                btnLogout.Enabled = true;
            }
        }
    }
}