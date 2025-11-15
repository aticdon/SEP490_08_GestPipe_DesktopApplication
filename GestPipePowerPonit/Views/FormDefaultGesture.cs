using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace GestPipePowerPonit
{
    public partial class FormDefaultGesture : Form
    {
        private DefaultGestureService _gestureService = new DefaultGestureService();
        private HomeUser _homeForm;
        private List<DefaultGestureDto> gestures;
        private readonly ApiClient _apiClient;
        private readonly string _currentUserId = Properties.Settings.Default.UserId;

        // ✅ THÊM AuthService
        private readonly AuthService _authService;

        public FormDefaultGesture(HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += FormDefaultGesture_Load;
            _homeForm = homeForm;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;

            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");

            // ✅ GẮN SỰ KIỆN LOGOUT VÀ PROFILE
            btnLogout.Click += btnLogout_Click;
            btnProfile.Click += btnProfile_Click;

            _apiClient = new ApiClient("https://localhost:7219");

            // ✅ KHỞI TẠO AuthService
            _authService = new AuthService();

            CultureManager.CultureChanged += async (s, e) =>
            {
                ApplyLanguage(CultureManager.CurrentCultureCode);
                await LoadDefaultGesturesAsync();
            };
        }

        private async void FormDefaultGesture_Load(object sender, EventArgs e)
        {
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            await LoadDefaultGesturesAsync();
        }

        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                gestures = await _gestureService.GetDefaultGesturesAsync();
                guna2DataGridView1.Rows.Clear();

                foreach (var g in gestures)
                {
                    guna2DataGridView1.Rows.Add(
                        I18nHelper.GetLocalized(g.Name),
                        I18nHelper.GetLocalized(g.Type),
                        $"{g.Accuracy * 100:F1}%",   // Hiển thị phần trăm
                        I18nHelper.GetLocalized(g.Status),
                        g.LastUpdate.ToString("dd-MM-yyyy"),
                        Properties.Resources.icon_view
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể tải danh sách gesture!\n" + ex.Message);
            }
        }

        private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index && e.RowIndex >= 0)
            {
                var basic = gestures[e.RowIndex];
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);

                if (detail == null)
                {
                    MessageBox.Show("Cannot get gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string description = _gestureService.GetGestureDescription(detail);
                string instruction = _gestureService.GetInstructionTable(detail);

                var detailForm = new FormGestureDetails(
                    I18nHelper.GetLocalized(detail.Name),
                    I18nHelper.GetLocalized(detail.Type),
                    $"{detail.Accuracy * 100:F1}%",
                    I18nHelper.GetLocalized(detail.Status),
                    detail.LastUpdate.ToString("dd-MM-yyyy"),
                    description,
                    instruction
                );
                detailForm.ShowDialog();
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Hide();
        }

        private void UpdateCultureAndApply(string cultureCode)
        {
            CultureManager.CurrentCultureCode = cultureCode;
            _apiClient.SetUserLanguageAsync(_currentUserId, cultureCode);
        }

        private void ApplyLanguage(string cultureCode)
        {
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnVersion.Text = Properties.Resources.Btn_Version;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnTrainingGesture.Text = Properties.Resources.Btn_Training;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            // Add any extra controls below if needed
            guna2DataGridView1.Columns["ColumnName"].HeaderText = Properties.Resources.Col_Name;
            guna2DataGridView1.Columns["ColumnAction"].HeaderText = Properties.Resources.Col_Type;
            guna2DataGridView1.Columns["ColumnAccuracy"].HeaderText = Properties.Resources.Col_Accuracy;
            guna2DataGridView1.Columns["ColumnStatus"].HeaderText = Properties.Resources.Col_Status;
            guna2DataGridView1.Columns["ColumnLastUpdate"].HeaderText = Properties.Resources.Col_LastUpdate;
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }

        private void btnTrainingGesture_Click(object sender, EventArgs e)
        {
            FormUserGesture uGestureForm = new FormUserGesture(_homeForm);
            uGestureForm.Show();
            this.Hide();
        }

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(_homeForm);
            form1.Show();
            this.Hide();
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
                btnProfile.Enabled = false;
                Cursor = Cursors.WaitCursor;

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[FormDefaultGesture] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[FormDefaultGesture] ✅ Logout successful");

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    var loginForm = new LoginForm();

                    // Đóng HomeUser nếu đang mở
                    _homeForm?.Close();

                    // Đóng form hiện tại
                    this.Hide();

                    // Show LoginForm
                    loginForm.Show();

                    // Dispose form hiện tại
                    this.Dispose();

                    Console.WriteLine("[FormDefaultGesture] ✅ Returned to LoginForm");
                    Console.WriteLine(new string('=', 60) + "\n");
                }
                else
                {
                    Console.WriteLine($"[FormDefaultGesture] ❌ Logout failed: {response?.Message}");

                    CustomMessageBox.ShowError(
                        response?.Message ?? Properties.Resources.Message_LogoutFailed,
                        Properties.Resources.Title_Error
                    );

                    btnLogout.Enabled = true;
                    btnProfile.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FormDefaultGesture] ❌ Exception: {ex.Message}");

                CustomMessageBox.ShowError(
                    $"{Properties.Resources.Message_LogoutError}: {ex.Message}",
                    Properties.Resources.Title_ConnectionError
                );

                btnLogout.Enabled = true;
                btnProfile.Enabled = true;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // ✅ THÊM PROFILE HANDLER
        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ Cách 2: Truyền this (Form) thay vì _homeForm (HomeUser)
                ProfileForm profileForm = new ProfileForm(_currentUserId, this);

                this.Hide();

                profileForm.Show();

                profileForm.FormClosed += (s, args) =>
                {
                    // Quay lại FormDefaultGesture sau khi đóng Profile
                    this.Show();
                };
            }
            catch (Exception ex)
            {
                string errorMessage = CultureManager.CurrentCultureCode == "vi-VN"
                    ? $"Không thể mở trang profile: {ex.Message}"
                    : $"Cannot open profile page: {ex.Message}";

                CustomMessageBox.ShowError(
                    errorMessage,
                    Properties.Resources.Title_Error
                );
            }
        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            FormUserGestureCustom uGestureForm = new FormUserGestureCustom(_homeForm);
            uGestureForm.Show();
            this.Hide();
        }
    }
}