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
    public partial class ListDefaultGestureForm : Form
    {
        private DefaultGestureService _gestureService = new DefaultGestureService();
        private UserGestureConfigService _uGestureService = new UserGestureConfigService();
        private HomeUser _homeForm;
        private List<DefaultGestureDto> gestures;
        private List<UserGestureConfigDto> uGestures;
        private readonly ApiClient _apiClient;
        private readonly string _currentUserId = Properties.Settings.Default.UserId;
        private enum GestureListType { Default, User }
        private GestureListType currentListType = GestureListType.Default;

        private bool isShowingUserGestures = false;

        // ✅ THÊM AuthService
        private readonly AuthService _authService;

        public ListDefaultGestureForm(HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += ListDefaultGestureForm_Load;
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

        private async void ListDefaultGestureForm_Load(object sender, EventArgs e)
        {
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            await LoadGesturesAsync();
        }

        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                currentListType = GestureListType.Default;
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
                        Properties.Resources.icon_view,
                        Properties.Resources.icon_traininggesture
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể tải danh sách gesture!\n" + ex.Message);
            }
        }

        private async Task LoadUserGesturesAsync()
        {
            try
            {
                currentListType = GestureListType.User;
                uGestures = await _uGestureService.GetUserGesturesAsync(_currentUserId);
                guna2DataGridView1.Rows.Clear();

                foreach (var g in uGestures)
                {
                    guna2DataGridView1.Rows.Add(
                        I18nHelper.GetLocalized(g.Name),
                        I18nHelper.GetLocalized(g.Type),
                        $"{g.Accuracy * 100:F1}%",   // Hiển thị phần trăm
                        I18nHelper.GetLocalized(g.Status),
                        g.LastUpdate.ToString("dd-MM-yyyy"),
                        Properties.Resources.icon_view,
                        Properties.Resources.icon_traininggesture
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách gesture!\n" + ex.Message);
            }
        }
        private async Task LoadGesturesAsync()
        {
            try
            {
                // Kiểm tra xem user có gesture config không
                uGestures = await _uGestureService.GetUserGesturesAsync(_currentUserId);

                if (uGestures != null && uGestures.Count > 0)
                {
                    // Có user gestures -> hiển thị user gestures
                    isShowingUserGestures = true;
                    await LoadUserGesturesAsync();
                    Console.WriteLine($"[LoadGestures] Hiển thị User Gestures ({uGestures.Count} items)");
                }
                else
                {
                    // Không có user gestures -> hiển thị default gestures
                    isShowingUserGestures = false;
                    await LoadDefaultGesturesAsync();
                    Console.WriteLine("[LoadGestures] Hiển thị Default Gestures");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadGestures] Lỗi: {ex.Message}");
                // Nếu có lỗi, fallback về default gestures
                isShowingUserGestures = false;
                await LoadDefaultGesturesAsync();
            }
        }
        private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Xử lý nút View
            if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index)
            {
                await HandleViewClick(e.RowIndex);
            }
            // Xử lý nút Training
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnTraining"].Index)
            {
                await HandleTrainingClick(e.RowIndex);
            }
        }

        private async Task HandleViewClick(int rowIndex)
        {
            try
            {
                if (isShowingUserGestures)
                {
                    var basic = uGestures[rowIndex];
                    var detail = await _uGestureService.GetGestureDetailAsync(basic.Id);
                    if (detail == null)
                    {
                        MessageBox.Show("Cannot get user gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string description = _uGestureService.GetGestureDescription(detail);
                    string instruction = _uGestureService.GetInstructionTable(detail);

                    var detailForm = new DetailGestureForm(
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
                else
                {
                    var basic = gestures[rowIndex];
                    var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                    if (detail == null)
                    {
                        MessageBox.Show("Cannot get gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string description = _gestureService.GetGestureDescription(detail);
                    string instruction = _gestureService.GetInstructionTable(detail);

                    var detailForm = new DetailGestureForm(
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing gesture details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task HandleTrainingClick(int rowIndex)
        {
            try
            {
                object detail = null;

                if (isShowingUserGestures)
                {
                    var basic = uGestures[rowIndex];
                    detail = await _uGestureService.GetGestureDetailAsync(basic.Id);
                }
                else
                {
                    var basic = gestures[rowIndex];
                    detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                }

                if (detail == null)
                {
                    MessageBox.Show("Cannot get gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ Xử lý chung cho cả hai loại gesture
                var gestureDetail = detail as dynamic;

                Bitmap arrowImg = null;
                string directionStr = "";
                string typeName = I18nHelper.GetLocalized(gestureDetail.Type);

                if (typeName == I18nHelper.GetString("Static", "Tĩnh"))
                {
                    arrowImg = Properties.Resources.staticImg;
                    directionStr = I18nHelper.GetString("Stand Still", "Đứng yên");
                }
                else
                {
                    if (gestureDetail.VectorData.MainAxisX == 1)
                    {
                        if (gestureDetail.VectorData.DeltaX > 0)
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
                    else if (gestureDetail.VectorData.MainAxisY == 1)
                    {
                        if (gestureDetail.VectorData.DeltaY > 0)
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

                var trainingForm = new IntructionTraingForm(
                    gestureDetail.VectorData.Fingers,
                    arrowImg,
                    I18nHelper.GetLocalized(gestureDetail.Name),
                    gestureDetail.PoseLabel,
                    I18nHelper.GetLocalized(gestureDetail.Type),
                    directionStr,
                    this,
                    isShowingUserGestures // ✅ Truyền flag để biết đang xử lý loại nào
                );

                trainingForm.GestureDetail = gestureDetail;
                trainingForm.SetDirectionText(directionStr);
                trainingForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening training form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Hide();
        }

        private async void UpdateCultureAndApply(string cultureCode)
        {
            try
            {

                CultureManager.CurrentCultureCode = cultureCode;
                ResourceHelper.SetCulture(cultureCode, this);

                await _apiClient.SetUserLanguageAsync(_currentUserId, cultureCode);
            }
            catch (Exception ex)
            {
                //CustomMessageBox.ShowError(
                //    Properties.Resources.Message_ChangeLanguageFailed,
                //    Properties.Resources.Title_Error
                //);
            }
        }

        private void ApplyLanguage(string cultureCode)
        {
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
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

        //private void btnTrainingGesture_Click(object sender, EventArgs e)
        //{
        //    FormUserGesture uGestureForm = new FormUserGesture(_homeForm);
        //    uGestureForm.Show();
        //    this.Hide();
        //}

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            PresentationForm form1 = new PresentationForm(_homeForm);
            form1.Show();
            this.Hide();
        }

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
                ProfileForm profileForm = new ProfileForm(_currentUserId, _homeForm);

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
        }
        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            ListRequestGestureForm uGestureForm = new ListRequestGestureForm(_homeForm);
            uGestureForm.Show();
            this.Hide();
        }
        public async Task RefreshGesturesAsync()
        {
            await LoadGesturesAsync();
        }

        private void btnInstruction_Click(object sender, EventArgs e)
        {
            InstructionForm instructionForm = new InstructionForm(_homeForm);
            instructionForm.Show();
            this.Hide();
        }
    }
}