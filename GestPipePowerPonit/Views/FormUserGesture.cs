using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views.Profile;
using GestPipePowerPonit.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace GestPipePowerPonit
{
    public partial class FormUserGesture : Form
    {
        private UserGestureConfigService _gestureService = new UserGestureConfigService();
        private HomeUser _homeForm;
        private List<UserGestureConfigDto> gestures;
        private string userId = Properties.Settings.Default.UserId;
        private readonly ApiClient _apiClient;
        private readonly AuthService _authService;

        //public FormUserGesture(HomeUser homeForm, string userId)
        public FormUserGesture(HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += FormUserGesture_Load;
            _homeForm = homeForm;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;

            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");

            // ✅ THÊM 2 DÒNG NÀY
            btnLogout.Click += btnLogout_Click;
            btnProfile.Click += btnProfile_Click;

            _apiClient = new ApiClient("https://localhost:7219");
            _authService = new AuthService(); // ✅ KHỞI TẠO AuthService

            CultureManager.CultureChanged += async (s, e) =>
            {
                ApplyLanguage(CultureManager.CurrentCultureCode);
                await LoadDefaultGesturesAsync();
            };
        }

        private async void FormUserGesture_Load(object sender, EventArgs e)
        {
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            await LoadDefaultGesturesAsync();
        }

        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                gestures = await _gestureService.GetUserGesturesAsync(userId);
                guna2DataGridView1.Rows.Clear();

                foreach (var g in gestures)
                {
                    guna2DataGridView1.Rows.Add(
                        I18nHelper.GetLocalized(g.Name),
                        I18nHelper.GetLocalized(g.Type),
                        $"{g.Accuracy * 100:F1}%",   // Hiển thị phần trăm
                        I18nHelper.GetLocalized(g.Status),
                        g.LastUpdate.ToString("dd-MM-yyyy"),
                        Properties.Resources.icon_view,
                        Properties.Resources.icon_traininggesture,
                        Properties.Resources.CustomCamera
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách gesture!\n" + ex.Message);
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
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnCustom"].Index && e.RowIndex >= 0)
            {
                // Lấy thông tin gesture nếu cần
                var basic = gestures[e.RowIndex];
                // Mở FormCustomGesture, truyền HomeUser (hoặc truyền gì bạn cần)
                var customForm = new FormCustomGesture(_homeForm);
                customForm.Show();
                // Nếu muốn, có thể hide form hiện tại: this.Hide();
                this.Hide();
            }
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnTraining"].Index && e.RowIndex >= 0)
            {
                var basic = gestures[e.RowIndex];
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                // ... Sau khi lấy detail xong
                Bitmap arrowImg = null;
                string directionStr = "";
                string typeName = I18nHelper.GetLocalized(detail.Type);
                if (typeName == I18nHelper.GetString("Static", "Tĩnh"))
                {
                    arrowImg = Properties.Resources.handlestaticImg;
                    directionStr = I18nHelper.GetString("Stand Still", "Đứng yên");
                }
                else
                {
                    if (detail.VectorData.MainAxisX == 1)
                    {
                        if (detail.VectorData.DeltaX > 0)
                        {
                            arrowImg = Properties.Resources.Left_to_right;
                            directionStr = I18nHelper.GetString("Left to Right", "Trái sang phải");
                        }
                        else
                        {
                            arrowImg = Properties.Resources.Right_to_left;
                            directionStr = I18nHelper.GetString("Right to Left", "Phải sang trái");
                        }
                    }
                    else if (detail.VectorData.MainAxisY == 1)
                    {
                        if (detail.VectorData.DeltaY > 0)
                        {
                            arrowImg = Properties.Resources.Top_to_bottom;
                            directionStr = I18nHelper.GetString("Top to Bottom", "Trên xuống dưới");
                        }
                        else
                        {
                            arrowImg = Properties.Resources.Bottom_to_top;
                            directionStr = I18nHelper.GetString("Bottom to Top", "Dưới lên trên");
                        }
                    }
                }

                // Truyền tất cả sang form instruction, cùng một nguồn!
                var trainingForm = new FormInstructionTraining(
                    detail.VectorData.Fingers,
                    arrowImg,
                    I18nHelper.GetLocalized(detail.Name),
                    detail.PoseLabel,
                    I18nHelper.GetLocalized(detail.Type),
                    directionStr, // truyền string direction
                    this
                );

                trainingForm.GestureDetail = detail;
                trainingForm.SetDirectionText(directionStr); // -> bạn thêm hàm public void SetDirectionText(string txt) { lblDirectionValue.Text = txt; }

                trainingForm.Show();
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
            _apiClient.SetUserLanguageAsync(userId, cultureCode);
        }
        private void ApplyLanguage(string cultureCode)
        {
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnVersion.Text = Properties.Resources.Btn_Version;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomeGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            // Add any extra controls below if needed
            guna2DataGridView1.Columns["ColumnName"].HeaderText = Properties.Resources.Col_Name;
            guna2DataGridView1.Columns["ColumnAction"].HeaderText = Properties.Resources.Col_Type;
            guna2DataGridView1.Columns["ColumnAccuracy"].HeaderText = Properties.Resources.Col_Accuracy;
            guna2DataGridView1.Columns["ColumnStatus"].HeaderText = Properties.Resources.Col_Status;
            guna2DataGridView1.Columns["ColumnLastUpdate"].HeaderText = Properties.Resources.Col_LastUpdate;
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
                Console.WriteLine("[FormUserGesture] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[FormUserGesture] ✅ Logout successful");

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    var loginForm = new Views.Auth.LoginForm();

                    // Đóng HomeUser nếu đang mở
                    _homeForm?.Close();

                    // Đóng form hiện tại
                    this.Hide();

                    // Show LoginForm
                    loginForm.Show();

                    // Dispose form hiện tại
                    this.Dispose();

                    Console.WriteLine("[FormUserGesture] ✅ Returned to LoginForm");
                    Console.WriteLine(new string('=', 60) + "\n");
                }
                else
                {
                    Console.WriteLine($"[FormUserGesture] ❌ Logout failed: {response?.Message}");

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
                Console.WriteLine($"[FormUserGesture] ❌ Exception: {ex.Message}");

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
                ProfileForm profileForm = new ProfileForm(userId, this);

                this.Hide();

                profileForm.Show();

                profileForm.FormClosed += (s, args) =>
                {
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
        }
        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }

        private void btnGestureControl_Click(object sender, EventArgs e)
        {
            FormDefaultGesture dGestureForm = new FormDefaultGesture(_homeForm);
            dGestureForm.Show();
            this.Hide();
        }

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(_homeForm);
            form1.Show();
            this.Hide();
        }
    }
}