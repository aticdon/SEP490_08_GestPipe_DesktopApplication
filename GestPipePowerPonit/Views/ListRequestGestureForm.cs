using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GestPipePowerPonit
{
    public partial class ListRequestGestureForm : Form
    {
        private DefaultGestureService _gestureService = new DefaultGestureService();
        private UserGestureConfigService _uGestureService = new UserGestureConfigService();
        private HomeUser _homeForm;

        // ✅ THAY ĐỔI: Có 2 danh sách riêng biệt
        private List<DefaultGestureDto> defaultGestures;
        private List<UserGestureConfigDto> userGestures;

        private string userId = Properties.Settings.Default.UserId;
        private readonly ApiClient _apiClient;
        private UserService _userService = new UserService();
        private ProfileService _profileService = new ProfileService();
        private AuthService _authService = new AuthService();

        private bool _canRequest;

        // ✅ THÊM: Flag để biết đang hiển thị loại nào
        private bool isShowingUserGestures = false;

        public ListRequestGestureForm(HomeUser homeForm)
        {
            InitializeComponent();
            _homeForm = homeForm;
            _apiClient = new ApiClient("https://localhost:7219");

            // ✅ Đăng ký events SAU KHI InitializeComponent
            this.Load += ListRequestGestureForm_Load;

            if (guna2DataGridView1 != null)
                guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;

            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");

            CultureManager.CultureChanged += async (s, e) =>
            {
                try
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.Invoke(new Action(() => ApplyLanguage(CultureManager.CurrentCultureCode)));
                        await LoadGesturesAsync(); // ✅ THAY ĐỔI: Gọi method mới
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CultureChanged] Error: {ex.Message}");
                }
            };
        }

        private async void ListRequestGestureForm_Load(object sender, EventArgs e)
        {
            try
            {
                ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);

                _canRequest = await _userService.CheckCanRequestAsync(userId);
                if (!_canRequest && lblRequestStatus != null)
                {
                    //// Đặt text song ngữ tùy theo ngôn ngữ hiện tại
                    //if (CultureManager.CurrentCultureCode.Contains("vi"))
                    //{
                    //    lblRequestStatus.Text = "Cử chỉ đang được huấn luyện. Vui lòng đợi hoàn thành để tiếp tục yêu cầu mới.";
                    //}
                    //else
                    //{
                    //    lblRequestStatus.Text = "Gesture is being trained. Please wait until it completes to continue.";
                    //}
                    lblRequestStatus.Text = I18nHelper.GetString(
                        "Gesture is being trained. Please wait until it completes to continue!",
                        "Cử chỉ đang được huấn luyện. Vui lòng đợi hoàn thành để tiếp tục yêu cầu mới!"
                    );
                    lblRequestStatus.Visible = true;
                }
                else if (lblRequestStatus != null)
                {
                    lblRequestStatus.Text = "";
                    lblRequestStatus.Visible = false;
                }

                if (btnRequest != null)
                {
                    btnRequest.Enabled = _canRequest;
                    btnRequest.ForeColor = _canRequest ? Color.White : Color.Black;
                }

                await LoadGesturesAsync(); // ✅ THAY ĐỔI: Gọi method mới
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Form_Load] Error: {ex.Message}");
                MessageBox.Show($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ METHOD MỚI: Tự động quyết định hiển thị loại gesture nào
        private async Task LoadGesturesAsync()
        {
            try
            {
                // Kiểm tra xem user có gesture config không
                userGestures = await _uGestureService.GetUserGesturesAsync(userId);

                if (userGestures != null && userGestures.Count > 0)
                {
                    // Có user gestures -> chỉ hiển thị user gestures
                    isShowingUserGestures = true;
                    await LoadUserGesturesAsync();
                    Console.WriteLine($"[LoadGestures] ✅ Hiển thị CHÍNH UserGestureConfig ({userGestures.Count} items)");
                }
                else
                {
                    // Không có user gestures -> chỉ hiển thị default gestures
                    isShowingUserGestures = false;
                    await LoadDefaultGesturesAsync();
                    Console.WriteLine("[LoadGestures] ✅ Hiển thị DefaultGesture (fallback)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadGestures] ❌ Lỗi: {ex.Message}");
                // Nếu có lỗi, fallback về default gestures
                isShowingUserGestures = false;
                await LoadDefaultGesturesAsync();
            }
        }

        // ✅ Load default gestures (chỉ khi không có UserGestureConfig)
        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                if (panelLoading != null)
                {
                    panelLoading.Visible = true;
                    panelLoading.BringToFront();
                }

                if (lblLoading != null)
                    lblLoading.Text = Properties.Resources.List_Loading;

                defaultGestures = await _gestureService.GetDefaultGesturesAsync();

                if (guna2DataGridView1 != null)
                {
                    guna2DataGridView1.Rows.Clear();
                    guna2DataGridView1.AllowUserToAddRows = false;
                }

                var requestService = new UserGestureRequestService();

                // Song song lấy requests cho tốc độ nhanh
                //var requestTasks = defaultGestures.Select(config => requestService.GetLatestRequestByConfigAsync(config.Id, userId)).ToList();
                //var requests = await Task.WhenAll(requestTasks);

                var configIds = defaultGestures.Select(config => config.Id).ToList();
                var requests = await requestService.GetLatestRequestsBatchAsync(userId, configIds);
                // Map thành Dictionary cho dễ tra cứu:
                var requestDict = requests?.ToDictionary(r => r.UserGestureConfigId, r => r) ?? new Dictionary<string, UserGestureRequestDto>();

                var rowsToAdd = new List<object[]>();
                for (int i = 0; i < defaultGestures.Count; i++)
                {
                    var config = defaultGestures[i];
                    //var request = requests[i];
                    //var requestDictEntry = requestDict.TryGetValue(config.Id, out var request) ? request : null;
                    var request = requestDict.TryGetValue(config.Id, out var req) ? req : null;
                    string status;
                    string statusToShow = "", timeToShow, accuracToShow;
                    object viewIcon, customIcon;

                    if (!_canRequest)
                    {
                        status = I18nHelper.GetLocalized(config.Status);

                        if (status.Contains("Active"))
                        {
                            statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
                        }
                        else
                        {
                            statusToShow = status;
                        }
                        timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = $"{config.Accuracy * 100:F1}%";
                        viewIcon = Properties.Resources.eye_gray;
                        customIcon = Properties.Resources.CustomCameraGray;
                    }
                    else if (request != null)
                    {
                        //status = I18nHelper.GetLocalized(request.Status);
                        status = I18nHelper.GetLocalized(request.Status);
                        if (status.Contains("Active"))
                        {
                            statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
                        }
                        else
                        {
                            statusToShow = status;
                        }
                        timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = "N/A";
                        viewIcon = Properties.Resources.eye_gray;
                        customIcon = Properties.Resources.CustomCameraGray;
                    }
                    else
                    {
                        status = I18nHelper.GetLocalized(config.Status);
                        if (status.Contains("Active"))
                        {
                            statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
                        }
                        else
                        {
                            statusToShow = status;
                        }
                        timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = $"{config.Accuracy * 100:F1}%";
                        viewIcon = Properties.Resources.eye;
                        customIcon = Properties.Resources.CustomCamera;
                    }

                    rowsToAdd.Add(new object[]
                    {
                        I18nHelper.GetLocalized(config.Name),
                        I18nHelper.GetLocalized(config.Type),
                        accuracToShow,
                        statusToShow,
                        timeToShow,
                        viewIcon,
                        customIcon
                    });
                }

                // Add tất cả dòng vào grid một lượt
                if (guna2DataGridView1 != null)
                {
                    foreach (var row in rowsToAdd)
                    {
                        guna2DataGridView1.Rows.Add(row);
                    }
                }

                Console.WriteLine($"[LoadDefaultGestures] ✅ Loaded {defaultGestures.Count} default gestures");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách default gesture!\n" + ex.Message);
            }
            finally
            {
                if (panelLoading != null)
                    panelLoading.Visible = false;
            }
        }

        private async Task LoadUserGesturesAsync()
        {
            try
            {
                if (panelLoading != null)
                {
                    panelLoading.Visible = true;
                    panelLoading.BringToFront();
                }

                if (lblLoading != null)
                    lblLoading.Text = Properties.Resources.List_Loading;

                userGestures = await _uGestureService.GetUserGesturesAsync(userId);

                if (guna2DataGridView1 != null)
                {
                    guna2DataGridView1.Rows.Clear();
                    guna2DataGridView1.AllowUserToAddRows = false;
                }

                var requestService = new UserGestureRequestService();

                // **Sửa ở đây: dùng batch thay cho từng request**
                var configIds = userGestures.Select(config => config.Id).ToList();
                var requests = await requestService.GetLatestRequestsBatchAsync(userId, configIds);
                var requestDict = requests?.ToDictionary(r => r.UserGestureConfigId, r => r) ?? new Dictionary<string, UserGestureRequestDto>();

                var rowsToAdd = new List<object[]>();
                for (int i = 0; i < userGestures.Count; i++)
                {
                    var config = userGestures[i];
                    var request = requestDict.TryGetValue(config.Id, out var req) ? req : null;
                    string status;
                    string statusToShow = "", timeToShow, accuracToShow;
                    object viewIcon, customIcon;

                    if (!_canRequest)
                    {
                        status = I18nHelper.GetLocalized(config.Status);

                        if (status.Contains("Active"))
                        {
                            statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
                        }
                        else
                        {
                            statusToShow = status;
                        }
                        timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = $"{config.Accuracy * 100:F1}%";
                        viewIcon = Properties.Resources.eye_gray;
                        customIcon = Properties.Resources.CustomCameraGray;
                    }
                    else if (request != null)
                    {
                        status = I18nHelper.GetLocalized(request.Status);
                        if (status.Contains("Active"))
                        {
                            statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
                        }
                        else
                        {
                            statusToShow = status;
                        }
                        timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = "N/A";
                        viewIcon = Properties.Resources.eye_gray;
                        customIcon = Properties.Resources.CustomCameraGray;
                    }
                    else
                    {
                        status = I18nHelper.GetLocalized(config.Status);
                        if (status.Contains("Active"))
                        {
                            statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
                        }
                        else
                        {
                            statusToShow = status;
                        }
                        timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = $"{config.Accuracy * 100:F1}%";
                        viewIcon = Properties.Resources.eye;
                        customIcon = Properties.Resources.CustomCamera;
                    }

                    rowsToAdd.Add(new object[]
                    {
                I18nHelper.GetLocalized(config.Name),
                I18nHelper.GetLocalized(config.Type),
                accuracToShow,
                statusToShow,
                timeToShow,
                viewIcon,
                customIcon
                    });
                }

                if (guna2DataGridView1 != null)
                {
                    foreach (var row in rowsToAdd)
                    {
                        guna2DataGridView1.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách user gesture!\n" + ex.Message);
            }
            finally
            {
                if (panelLoading != null)
                    panelLoading.Visible = false;
            }
        }
        //private async Task LoadUserGesturesAsync()
        //{
        //    try
        //    {
        //        if (panelLoading != null)
        //        {
        //            panelLoading.Visible = true;
        //            panelLoading.BringToFront();
        //        }

        //        if (lblLoading != null)
        //            lblLoading.Text = Properties.Resources.List_Loading;

        //        userGestures = await _uGestureService.GetUserGesturesAsync(userId);

        //        if (guna2DataGridView1 != null)
        //        {
        //            guna2DataGridView1.Rows.Clear();
        //            guna2DataGridView1.AllowUserToAddRows = false;
        //        }

        //        var requestService = new UserGestureRequestService();

        //        // Song song lấy requests cho tốc độ nhanh
        //        var requestTasks = userGestures.Select(config => requestService.GetLatestRequestByConfigAsync(config.Id, userId)).ToList();
        //        var requests = await Task.WhenAll(requestTasks);

        //        var rowsToAdd = new List<object[]>();
        //        for (int i = 0; i < userGestures.Count; i++)
        //        {
        //            var config = userGestures[i];
        //            var request = requests[i];
        //            string status;
        //            string statusToShow = "", timeToShow, accuracToShow;
        //            object viewIcon, customIcon;

        //            if (!_canRequest)
        //            {
        //                status = I18nHelper.GetLocalized(config.Status);

        //                if (status.Contains("Active"))
        //                {
        //                    statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
        //                }
        //                else
        //                {
        //                    statusToShow = status;
        //                }
        //                timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = $"{config.Accuracy * 100:F1}%";
        //                viewIcon = Properties.Resources.eye_gray;
        //                customIcon = Properties.Resources.CustomCameraGray;
        //            }
        //            else if (request != null)
        //            {
        //                status = I18nHelper.GetLocalized(request.Status);
        //                if (status.Contains("Active"))
        //                {
        //                    statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
        //                }
        //                else
        //                {
        //                    statusToShow = status;
        //                }
        //                timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = "N/A";
        //                viewIcon = Properties.Resources.eye_gray;
        //                customIcon = Properties.Resources.CustomCameraGray;
        //            }
        //            else
        //            {
        //                status = I18nHelper.GetLocalized(config.Status);
        //                if (status.Contains("Active"))
        //                {
        //                    statusToShow = I18nHelper.GetString("Ready", "Sẵn sàng");
        //                }
        //                else
        //                {
        //                    statusToShow = status;
        //                }
        //                timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = $"{config.Accuracy * 100:F1}%";
        //                viewIcon = Properties.Resources.eye;
        //                customIcon = Properties.Resources.CustomCamera;
        //            }

        //            rowsToAdd.Add(new object[]
        //            {
        //                I18nHelper.GetLocalized(config.Name),
        //                I18nHelper.GetLocalized(config.Type),
        //                accuracToShow,
        //                statusToShow,
        //                timeToShow,
        //                viewIcon,
        //                customIcon
        //            });
        //        }

        //        // Add tất cả dòng vào grid một lượt
        //        if (guna2DataGridView1 != null)
        //        {
        //            foreach (var row in rowsToAdd)
        //            {
        //                guna2DataGridView1.Rows.Add(row);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Không thể tải danh sách default gesture!\n" + ex.Message);
        //    }
        //    finally
        //    {
        //        if (panelLoading != null)
        //            panelLoading.Visible = false;
        //    }
        //}

        // ✅ THAY ĐỔI: Xử lý click dựa trên loại gesture đang hiển thị
        private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var columnView = guna2DataGridView1?.Columns["ColumnView"];
            var columnCustom = guna2DataGridView1?.Columns["ColumnCustom"];

            if (isShowingUserGestures)
            {
                // ✅ XỬ LÝ UserGestureConfig - luôn cho phép tương tác
                await HandleUserGestureClick(e.RowIndex, e.ColumnIndex, columnView, columnCustom);
            }
            else
            {
                // ✅ XỬ LÝ DefaultGesture - kiểm tra request như cũ
                await HandleDefaultGestureClick(e.RowIndex, e.ColumnIndex, columnView, columnCustom);
            }
        }

        // ✅ METHOD MỚI: Xử lý UserGesture click
        private async Task HandleUserGestureClick(int rowIndex, int columnIndex, DataGridViewColumn columnView, DataGridViewColumn columnCustom)
        {
            var basic = userGestures[rowIndex];
            var requestService = new UserGestureRequestService();
            var request = await requestService.GetLatestRequestByConfigAsync(basic.Id, userId);

            // Nếu là dòng có request thì KHÔNG xử lý View/Custom
            if (request != null &&
                ((columnView != null && columnIndex == columnView.Index) ||
                 (columnCustom != null && columnIndex == columnCustom.Index)))
            {
                return;
            }

            // Xử lý View
            if (columnView != null && columnIndex == columnView.Index)
            {
                var detail = await _uGestureService.GetGestureDetailAsync(basic.Id);
                if (detail == null) return;

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
            // Xử lý Custom
            else if (columnCustom != null && columnIndex == columnCustom.Index)
            {
                var detail = await _uGestureService.GetGestureDetailAsync(basic.Id);
                if (detail == null) return;

                string poseLabel = detail.PoseLabel;
                string userGesture = I18nHelper.GetLocalized(detail.Name);
                string userName = await GetUserNameAsync();
                string gestureId = basic.Id;

                var customForm = new CustomGestureForm(_homeForm, gestureId, userName, poseLabel, userGesture, true);
                customForm.Show();
                this.Hide();
            }
        }

        // ✅ METHOD MỚI: Xử lý DefaultGesture click (logic cũ)
        private async Task HandleDefaultGestureClick(int rowIndex, int columnIndex, DataGridViewColumn columnView, DataGridViewColumn columnCustom)
        {
            var basic = defaultGestures[rowIndex];

            // LẤY request cho row hiện tại
            var requestService = new UserGestureRequestService();
            var request = await requestService.GetLatestRequestByConfigAsync(basic.Id, userId);

            // Nếu là dòng có request thì KHÔNG xử lý View/Custom
            if (request != null &&
                ((columnView != null && columnIndex == columnView.Index) ||
                 (columnCustom != null && columnIndex == columnCustom.Index)))
            {
                return;
            }

            // Nếu không có request thì cho phép thao tác như cũ:
            if (columnView != null && columnIndex == columnView.Index)
            {
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                if (detail == null) return;

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
            else if (columnCustom != null && columnIndex == columnCustom.Index)
            {
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                if (detail == null) return;

                string poseLabel = detail.PoseLabel;
                string userGesture = I18nHelper.GetLocalized(detail.Name);
                string userName = await GetUserNameAsync();
                string gestureId = basic.Id;

                var customForm = new CustomGestureForm(_homeForm, gestureId, userName, poseLabel, userGesture, false);
                customForm.Show();
                this.Hide();
            }
        }

        // ✅ METHOD MỚI: Lấy tên user (tách riêng để tránh lặp code)
        private async Task<string> GetUserNameAsync()
        {
            string userName = "unknown";
            try
            {
                var profileResponse = await _profileService.GetProfileAsync(userId);

                if (profileResponse.Success && profileResponse.Data?.Profile != null)
                {
                    userName = profileResponse.Data.Profile.FullName;

                    // Nếu FullName rỗng, fallback sang email
                    if (string.IsNullOrWhiteSpace(userName) && profileResponse.Data.User != null)
                    {
                        userName = !string.IsNullOrWhiteSpace(profileResponse.Email)
                            ? profileResponse.Email
                            : profileResponse.UserId;
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ [FormRequestGestures] Failed to load profile: {profileResponse.Message}");
                    userName = $"User {userId.Substring(0, Math.Min(8, userId.Length))}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting user name: {ex.Message}");
                userName = $"User {userId.Substring(0, Math.Min(8, userId.Length))}";
            }

            return userName;
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

                await _apiClient.SetUserLanguageAsync(userId, cultureCode);

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
            try
            {
                ResourceHelper.SetCulture(cultureCode, this);

                if (btnHome != null)
                    btnHome.Text = Properties.Resources.Btn_Home;
                if (btnGestureControl != null)
                    btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
                if (btnInstruction != null)
                    btnInstruction.Text = Properties.Resources.Btn_Instruction;
                if (btnCustomGesture != null)
                    btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
                if (btnPresentation != null)
                    btnPresentation.Text = Properties.Resources.Btn_Present;
                if (btnRequest != null)
                    btnRequest.Text = Properties.Resources.Btn_RequestGesture;

                if (guna2DataGridView1?.Columns != null)
                {
                    var colName = guna2DataGridView1.Columns["ColumnName"];
                    if (colName != null)
                        colName.HeaderText = Properties.Resources.Col_Name;

                    var colAction = guna2DataGridView1.Columns["ColumnAction"];
                    if (colAction != null)
                        colAction.HeaderText = Properties.Resources.Col_Type;

                    var colAccuracy = guna2DataGridView1.Columns["ColumnAccuracy"];
                    if (colAccuracy != null)
                        colAccuracy.HeaderText = Properties.Resources.Col_Accuracy;

                    var colStatus = guna2DataGridView1.Columns["ColumnStatus"];
                    if (colStatus != null)
                        colStatus.HeaderText = Properties.Resources.Col_Status;

                    var colLastUpdate = guna2DataGridView1.Columns["ColumnLastUpdate"];
                    if (colLastUpdate != null)
                        colLastUpdate.HeaderText = Properties.Resources.Col_LastUpdate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApplyLanguage] Error: {ex.Message}");
            }
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }

        private void btnGestureControl_Click(object sender, EventArgs e)
        {
            ListDefaultGestureForm dGestureForm = new ListDefaultGestureForm(_homeForm);
            dGestureForm.Show();
            this.Hide();
        }

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            PresentationForm form1 = new PresentationForm(_homeForm);
            form1.Show();
            this.Hide();
        }

        private void btnTrainingGesture_Click(object sender, EventArgs e)
        {
            FormUserGesture uGestureForm = new FormUserGesture(_homeForm);
            uGestureForm.Show();
            this.Hide();
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            var requestForm = new RequestGestureForm(userId, isShowingUserGestures);
            requestForm.ShowDialog();
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

                if (btnLogout != null) btnLogout.Enabled = false;
                if (btnProfile != null) btnProfile.Enabled = false;
                Cursor = Cursors.WaitCursor;

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[ListRequestGestureForm] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[ListRequestGestureForm] ✅ Logout successful");

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    var loginForm = new LoginForm();
                    _homeForm?.Close();
                    this.Hide();
                    loginForm.Show();
                    this.Dispose();

                    Console.WriteLine("[ListRequestGestureForm] ✅ Returned to LoginForm");
                    Console.WriteLine(new string('=', 60) + "\n");
                }
                else
                {
                    Console.WriteLine($"[ListRequestGestureForm] ❌ Logout failed: {response?.Message}");

                    CustomMessageBox.ShowError(
                        response?.Message ?? Properties.Resources.Message_LogoutFailed,
                        Properties.Resources.Title_Error
                    );

                    if (btnLogout != null) btnLogout.Enabled = true;
                    if (btnProfile != null) btnProfile.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ListRequestGestureForm] ❌ Exception: {ex.Message}");

                CustomMessageBox.ShowError(
                    $"{Properties.Resources.Message_LogoutError}: {ex.Message}",
                    Properties.Resources.Title_ConnectionError
                );

                if (btnLogout != null) btnLogout.Enabled = true;
                if (btnProfile != null) btnProfile.Enabled = true;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileForm profileForm = new ProfileForm(userId, _homeForm);

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

        // ✅ THÊM METHOD PUBLIC để refresh dữ liệu khi cần
        public async Task RefreshGesturesAsync()
        {
            await LoadGesturesAsync();
        }
    }
}