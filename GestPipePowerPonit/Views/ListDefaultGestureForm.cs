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
            await LoadDefaultGesturesAsync();
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

        private async Task LoadGesturesAsync()
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
        private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Nếu đang hiển thị Default Gesture
            if (currentListType == GestureListType.Default)
            {
                // Xử lý nút View
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
                // Xử lý nút Training
                else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnTraining"].Index && e.RowIndex >= 0)
                {
                    var basic = gestures[e.RowIndex];
                    var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
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

                    var trainingForm = new FormInstructionTraining(
                        detail.VectorData.Fingers,
                        arrowImg,
                        I18nHelper.GetLocalized(detail.Name),
                        detail.PoseLabel,
                        I18nHelper.GetLocalized(detail.Type),
                        directionStr,
                        this
                    );

                    trainingForm.GestureDetail = detail;
                    trainingForm.SetDirectionText(directionStr);
                    trainingForm.Show();
                }
            }
            // Nếu đang hiển thị User Gesture
            else if (currentListType == GestureListType.User)
            {
                // Xử lý nút View
                if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index && e.RowIndex >= 0)
                {
                    var basic = uGestures[e.RowIndex];
                    var detail = await _uGestureService.GetGestureDetailAsync(basic.Id);
                    if (detail == null)
                    {
                        MessageBox.Show("Cannot get gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string description = _uGestureService.GetGestureDescription(detail);
                    string instruction = _uGestureService.GetInstructionTable(detail);

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
                // Xử lý nút Training
                else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnTraining"].Index && e.RowIndex >= 0)
                {
                    var basic = uGestures[e.RowIndex];
                    var detail = await _uGestureService.GetGestureDetailAsync(basic.Id);
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

                    //var trainingForm = new FormInstructionTraining(
                    //    detail.VectorData.Fingers,
                    //    arrowImg,
                    //    I18nHelper.GetLocalized(detail.Name),
                    //    detail.PoseLabel,
                    //    I18nHelper.GetLocalized(detail.Type),
                    //    directionStr,
                    //    this
                    //);
                    var trainingForm = new FormInstructionTraining(
                        detail.VectorData.Fingers,
                        arrowImg,
                        I18nHelper.GetLocalized(detail.Name),
                        detail.PoseLabel,
                        I18nHelper.GetLocalized(detail.Type),
                        directionStr,
                        this,
                        true // ✅ isUserGesture = true
                    );

                    trainingForm.GestureDetail = detail;
                    trainingForm.SetDirectionText(directionStr);
                    trainingForm.Show();
                }
            }
        }

        //private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index && e.RowIndex >= 0)
        //    {
        //        var basic = gestures[e.RowIndex];
        //        var detail = await _gestureService.GetGestureDetailAsync(basic.Id);

        //        if (detail == null)
        //        {
        //            MessageBox.Show("Cannot get gesture details!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        string description = _gestureService.GetGestureDescription(detail);
        //        string instruction = _gestureService.GetInstructionTable(detail);

        //        var detailForm = new FormGestureDetails(
        //            I18nHelper.GetLocalized(detail.Name),
        //            I18nHelper.GetLocalized(detail.Type),
        //            $"{detail.Accuracy * 100:F1}%",
        //            I18nHelper.GetLocalized(detail.Status),
        //            detail.LastUpdate.ToString("dd-MM-yyyy"),
        //            description,
        //            instruction
        //        );
        //        detailForm.ShowDialog();
        //    }
        //    else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnTraining"].Index && e.RowIndex >= 0)
        //    {
        //        var basic = gestures[e.RowIndex];
        //        var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
        //        // ... Sau khi lấy detail xong
        //        Bitmap arrowImg = null;
        //        string directionStr = "";
        //        string typeName = I18nHelper.GetLocalized(detail.Type);
        //        if (typeName == I18nHelper.GetString("Static", "Tĩnh"))
        //        {
        //            arrowImg = Properties.Resources.handlestaticImg;
        //            directionStr = I18nHelper.GetString("Stand Still", "Đứng yên");
        //        }
        //        else
        //        {
        //            if (detail.VectorData.MainAxisX == 1)
        //            {
        //                if (detail.VectorData.DeltaX > 0)
        //                {
        //                    arrowImg = Properties.Resources.Left_to_right;
        //                    directionStr = I18nHelper.GetString("Left to Right", "Trái sang phải");
        //                }
        //                else
        //                {
        //                    arrowImg = Properties.Resources.Right_to_left;
        //                    directionStr = I18nHelper.GetString("Right to Left", "Phải sang trái");
        //                }
        //            }
        //            else if (detail.VectorData.MainAxisY == 1)
        //            {
        //                if (detail.VectorData.DeltaY > 0)
        //                {
        //                    arrowImg = Properties.Resources.Top_to_bottom;
        //                    directionStr = I18nHelper.GetString("Top to Bottom", "Trên xuống dưới");
        //                }
        //                else
        //                {
        //                    arrowImg = Properties.Resources.Bottom_to_top;
        //                    directionStr = I18nHelper.GetString("Bottom to Top", "Dưới lên trên");
        //                }
        //            }
        //        }

        //        // Truyền tất cả sang form instruction, cùng một nguồn!
        //        var trainingForm = new FormInstructionTraining(
        //            detail.VectorData.Fingers,
        //            arrowImg,
        //            I18nHelper.GetLocalized(detail.Name),
        //            detail.PoseLabel,
        //            I18nHelper.GetLocalized(detail.Type),
        //            directionStr, // truyền string direction
        //            this
        //        );

        //        trainingForm.GestureDetail = detail;
        //        trainingForm.SetDirectionText(directionStr); // -> bạn thêm hàm public void SetDirectionText(string txt) { lblDirectionValue.Text = txt; }

        //        trainingForm.Show();
        //    }
        //}

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

        private void ApplyLanguage(string cultureCode)
        {
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            btnUserGesture.Text = Properties.Resources.BtnUserGesture;
            btnDefaultGesture.Text = Properties.Resources.BtnDefaultGesture;
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
            PresentationForm form1 = new PresentationForm(_homeForm);
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

        private async void btnDefaultGesture_Click(object sender, EventArgs e)
        {
            btnDefaultGesture.Enabled = false;
            btnUserGesture.Enabled = true;
            await LoadDefaultGesturesAsync();
        }

        private async void btnUserGesture_Click(object sender, EventArgs e)
        {
            btnDefaultGesture.Enabled = true;
            btnUserGesture.Enabled = false;
            await LoadGesturesAsync();
        }
    }
}