using GestPipe.GestPipePowerPoint.Models.DTOs;
using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private GestureDownloadService _gestureDownloadService;
        private GestureUploadService _gestureUploadService;
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
        private bool _canDownload;

        // ✅ THÊM: Flag để biết đang hiển thị loại nào
        private bool isShowingUserGestures = false;
        private int _spinnerAngle = 0;

        public ListRequestGestureForm(HomeUser homeForm)
        {
            InitializeComponent();
            _homeForm = homeForm;
            _apiClient = new ApiClient("https://localhost:7219");
            _gestureDownloadService = new GestureDownloadService(_uGestureService, _userService);
            _gestureUploadService = new GestureUploadService("https://localhost:7219");
            if (spinnerTimer != null)
                spinnerTimer.Tick += spinnerTimer_Tick;
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
                ResourceHelper.SetCulture(GestPipePowerPonit.CultureManager.CurrentCultureCode, this);

                ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);

                _canRequest = await _userService.CheckCanRequestAsync(userId);
                _canDownload = await _userService.CheckCanDownloadAsync(userId);
                if (!_canRequest && lblRequestStatus != null)
                {
                    lblRequestStatus.Text = I18nHelper.GetString(
                        "Gesture is being trained. Please wait until it completes to continue!",
                        "Cử chỉ đang được huấn luyện. Vui lòng đợi hoàn thành để tiếp tục!"
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
                if (btnDownload != null)
                {
                    btnDownload.Enabled = _canDownload;
                    btnDownload.Image = _canDownload
                                        ? Properties.Resources.icon_download           // trạng thái cho phép tải
                                        : Properties.Resources.icon_download_silver;
                }
                CenterLoadingPanel();
                await LoadGesturesAsync(); // ✅ THAY ĐỔI: Gọi method mới
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Form_Load] Error: {ex.Message}");
                Console.WriteLine($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                {
                    lblLoading.Text = Properties.Resources.List_Loading;
                    AlignLoadingControls();
                }
                defaultGestures = await _gestureService.GetDefaultGesturesAsync();

                if (guna2DataGridView1 != null)
                {
                    guna2DataGridView1.Rows.Clear();
                    guna2DataGridView1.AllowUserToAddRows = false;
                }

                var requestService = new UserGestureRequestService();


                var configIds = defaultGestures.Select(config => config.Id).ToList();
                var requests = await requestService.GetLatestRequestsBatchAsync(userId, configIds);
                var requestDict = requests?.ToDictionary(r => r.UserGestureConfigId, r => r) ?? new Dictionary<string, UserGestureRequestDto>();

                var rowsToAdd = new List<object[]>();
                for (int i = 0; i < defaultGestures.Count; i++)
                {
                    var config = defaultGestures[i];
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
                Console.WriteLine("Không thể tải danh sách default gesture!\n" + ex.Message);
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
                {
                    lblLoading.Text = Properties.Resources.List_Loading;
                    AlignLoadingControls();  // 👈 THÊM
                }
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
                Console.WriteLine("Không thể tải danh sách user gesture!\n" + ex.Message);
            }
            finally
            {
                if (panelLoading != null)
                    panelLoading.Visible = false;
            }
        }
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
            try
            {
                //ResourceHelper.SetCulture(cultureCode, this);

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
                btnDownload.Text = Properties.Resources.btnDownload;
                lblRequestStatus.Text = I18nHelper.GetString(
                        "Gesture is being trained. Please wait until it completes to continue!",
                        "Cử chỉ đang được huấn luyện. Vui lòng đợi hoàn thành để tiếp tục!"
                    );
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
        private void btnRequest_Click(object sender, EventArgs e)
        {
            var requestForm = new RequestGestureForm(userId, isShowingUserGestures);

            requestForm.FormClosed += async (s, args) =>
            {
                // Nếu user đã bấm Request thành công
                if (requestForm.RequestSentSuccessfully)
                {
                    // 👉 Hiện màn hình chờ + upload + hiện % file
                    await HandleUploadAfterRequestAsync();
                }

                // CẬP NHẬT TRẠNG THÁI request mới sau khi form request đóng
                _canRequest = await _userService.CheckCanRequestAsync(userId);
                _canDownload = await _userService.CheckCanDownloadAsync(userId);

                if (!_canRequest && lblRequestStatus != null)
                {
                    lblRequestStatus.Text = I18nHelper.GetString(
                        "Gesture is being trained. Please wait until it completes to continue!",
                        "Cử chỉ đang được huấn luyện. Vui lòng đợi hoàn thành để tiếp tục!"
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
                if (btnDownload != null)
                {
                    btnDownload.Enabled = _canDownload;
                    btnDownload.Image = _canDownload
                        ? Properties.Resources.icon_download
                        : Properties.Resources.icon_download_silver;
                }

                await RefreshGesturesAsync();
            };

            requestForm.ShowDialog();
        }

        private void UpdateUploadProgress(int uploaded, int total)
        {
            if (lblLoading == null) return;

            double percent = total > 0 ? uploaded * 100.0 / total : 0;

            string textEn = $"Uploading gesture data to Google Drive...\nFiles: {uploaded}/{total} ({percent:0}%)";
            string textVi = $"Đang gửi dữ liệu cử chỉ lên Google Drive...\nFile: {uploaded}/{total} ({percent:0}%)";

            lblLoading.Text = I18nHelper.GetString(textEn, textVi);
            AlignLoadingControls();
            lblLoading.Refresh();
        }
        private async Task<bool> UploadUserGesturesWithProgressAsync()
        {
            try
            {
                // Message đầu tiên cho overlay
                UpdateDownloadMessage(
                    "Uploading gesture data to Google Drive...",
                    "Đang gửi dữ liệu cử chỉ lên Google Drive..."
                );

                // Gọi service: start upload + poll tiến độ giống download
                var hasFiles = await _gestureUploadService.UploadUserGesturesWithProgressAsync(
                    userId,
                    (uploaded, total) =>
                    {
                        // callback chạy trên thread nền ⇒ cần Invoke sang UI thread
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    UpdateUploadProgress(uploaded, total);
                                }));
                            }
                            catch
                            {
                                // Form có thể đã dispose khi đang upload, cứ bỏ qua
                            }
                        }
                    });

                if (!hasFiles)
                {
                    UpdateDownloadMessage(
                        "No gesture files to upload.",
                        "Không có file cử chỉ nào để gửi."
                    );
                }
                return hasFiles;
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError(ex.Message, "Upload error");
                return false;
            }
        }

        private async Task HandleUploadAfterRequestAsync()
        {
            try
            {
                // Dùng lại overlay download
                ShowDownloadLoading();

                // Disable toàn bộ nút giống lúc download
                btnLanguageEN.Enabled = false;
                btnLanguageVN.Enabled = false;
                btnPresentation.Enabled = false;
                btnCustomGesture.Enabled = false;
                btnGestureControl.Enabled = false;
                btnHome.Enabled = false;
                btnInstruction.Enabled = false;
                btnProfile.Enabled = false;
                btnLogout.Enabled = false;
                btnDownload.Enabled = false;
                btnRequest.Enabled = false;

                var uploadOk = await UploadUserGesturesWithProgressAsync();

                if (uploadOk)
                {
                    CustomMessageBox.ShowSuccess(
                        I18nHelper.GetString(
                            "Uploaded gesture files to server successfully.",
                            "Đã gửi file cử chỉ lên máy chủ thành công."
                        ),
                        I18nHelper.GetString("Upload gesture", "Gửi cử chỉ")
                    );
                    _canDownload = await _userService.CheckCanDownloadAsync(userId);
                    _canRequest = await _userService.CheckCanRequestAsync(userId);

                    Console.WriteLine($"[Upload] ✅ Updated status: canDownload={_canDownload}, canRequest={_canRequest}");
                }
            }
            finally
            {
                HideDownloadLoading();
                await UpdateButtonStatesAsync();

                btnLanguageEN.Enabled = true;
                btnLanguageVN.Enabled = true;
                btnPresentation.Enabled = true;
                btnCustomGesture.Enabled = true;
                btnGestureControl.Enabled = true;
                btnHome.Enabled = true;
                btnInstruction.Enabled = true;
                btnProfile.Enabled = true;
                btnLogout.Enabled = true;
            }
        }
        /// <summary>
        /// Cập nhật trạng thái nút Download/Request từ server và hiển thị UI
        /// </summary>
        private async Task UpdateButtonStatesAsync()
        {
            // 1.  Lấy trạng thái mới từ server
            _canDownload = await _userService.CheckCanDownloadAsync(userId);
            _canRequest = await _userService.CheckCanRequestAsync(userId);

            Console.WriteLine($"[UpdateButtonStates] canDownload={_canDownload}, canRequest={_canRequest}");

            // 2. Cập nhật nút Download
            if (btnDownload != null)
            {
                btnDownload.Enabled = _canDownload;
                btnDownload.Image = _canDownload
                    ? Properties.Resources.icon_download
                    : Properties.Resources.icon_download_silver;
            }

            // 3. Cập nhật nút Request
            if (btnRequest != null)
            {
                btnRequest.Enabled = _canRequest;
                btnRequest.ForeColor = _canRequest ? Color.White : Color.Black;
            }

            // 4. Cập nhật label cảnh báo
            if (lblRequestStatus != null)
            {
                if (!_canRequest)
                {
                    lblRequestStatus.Text = I18nHelper.GetString(
                        "Gesture is being trained. Please wait until it completes to continue!",
                        "Cử chỉ đang được huấn luyện. Vui lòng đợi hoàn thành để tiếp tục!"
                    );
                    lblRequestStatus.Visible = true;
                }
                else
                {
                    lblRequestStatus.Text = "";
                    lblRequestStatus.Visible = false;
                }
            }
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
        private void spinnerTimer_Tick(object sender, EventArgs e)
        {
            _spinnerAngle += 15;
            if (_spinnerAngle >= 360) _spinnerAngle = 0;
            DrawSpinner();
        }

        private void DrawSpinner()
        {
            if (loadingSpinner == null)
                return;

            loadingSpinner.Image?.Dispose();

            Bitmap spinnerBitmap = new Bitmap(loadingSpinner.Width, loadingSpinner.Height);
            using (Graphics g = Graphics.FromImage(spinnerBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                int centerX = loadingSpinner.Width / 2;
                int centerY = loadingSpinner.Height / 2;
                int radius = 20;

                for (int i = 0; i < 8; i++)
                {
                    double angle = (_spinnerAngle + i * 45) * Math.PI / 180;
                    int x = centerX + (int)(Math.Cos(angle) * radius);
                    int y = centerY + (int)(Math.Sin(angle) * radius);

                    int alpha = 255 - (i * 30);
                    if (alpha < 0) alpha = 0;

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                    {
                        g.FillEllipse(brush, x - 3, y - 3, 6, 6);
                    }
                }
            }

            loadingSpinner.Image = spinnerBitmap;
        }

        // Helper song ngữ
        private string GetLocalizedText(string en, string vi)
        {
            return I18nHelper.GetString(en, vi);
        }

        // Hiện overlay loading khi Download
        private void ShowDownloadLoading()
        {
            if (panelLoading != null)
            {
                panelLoading.Visible = true;
                panelLoading.BringToFront();
            }

            if (lblLoading != null)
            {
                lblLoading.Text = GetLocalizedText(
                    "Downloading and importing gestures...\nPlease wait...",
                    "        Đang tải và import cử chỉ...   \n        Vui lòng đợi...   "
                );
                AlignLoadingControls();
            }

            _spinnerAngle = 0;
            DrawSpinner();
            spinnerTimer?.Start();

            if (btnDownload != null) btnDownload.Enabled = false;
            if (btnRequest != null) btnRequest.Enabled = false;

            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents(); // cập nhật UI ngay
        }

        // Tắt overlay loading
        private void HideDownloadLoading()
        {
            spinnerTimer?.Stop();

            if (panelLoading != null)
                panelLoading.Visible = false;

            if (btnDownload != null) btnDownload.Enabled = _canDownload;
            if (btnRequest != null) btnRequest.Enabled = _canRequest;

            this.Cursor = Cursors.Default;
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

        private void btnInstruction_Click(object sender, EventArgs e)
        {
            InstructionForm instructionForm = new InstructionForm(_homeForm);
            instructionForm.Show();
            this.Hide();
        }
        // Hiển thị message đơn giản
        private void UpdateDownloadMessage(string messageEn, string messageVi)
        {
            if (lblLoading == null) return;

            lblLoading.Text = I18nHelper.GetString(messageEn, messageVi);
            AlignLoadingControls();
            lblLoading.Refresh();
        }

        // Hiển thị tiến độ file: X/Y (%)
        private void UpdateFileDownloadProgress(int synced, int total)
        {
            Console.WriteLine($"[UpdateFileDownloadProgress] synced={synced}, total={total}");
            if (lblLoading == null) return;

            double percent = total > 0 ? synced * 100.0 / total : 0;

            string textEn = $"Downloading gesture data from Google Drive...\nFiles: {synced}/{total} ({percent:0}%)";
            string textVi = $"Đang tải dữ liệu cử chỉ từ Google Drive...\nFile: {synced}/{total} ({percent:0}%)";

            lblLoading.Text = I18nHelper.GetString(textEn, textVi);
            AlignLoadingControls();
            lblLoading.Refresh();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ShowDownloadLoading();

                btnLanguageEN.Enabled = false;
                btnLanguageVN.Enabled = false;
                btnPresentation.Enabled = false;
                btnCustomGesture.Enabled = false;
                btnGestureControl.Enabled = false;
                btnHome.Enabled = false;
                btnInstruction.Enabled = false;
                btnProfile.Enabled = false;
                btnLogout.Enabled = false;

                // ===== 1. SYNC FILE TỪ GOOGLE DRIVE =====
                UpdateDownloadMessage(
                    "Syncing files from Google Drive...",
                    "Đang đồng bộ file từ Google Drive..."
                );

                // Gọi sync và truyền callback để cập nhật realtime
                await _gestureDownloadService.SyncFromDriveAsync(
                    userId,
                    (synced, total) =>
                    {
                        // ⚠️ Callback chạy trên thread nền, phải Invoke sang UI thread
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    if (total > 0)
                                    {
                                        UpdateFileDownloadProgress(synced, total);
                                    }
                                    else
                                    {
                                        // Trường hợp không có file nào
                                        UpdateDownloadMessage(
                                            "No files found to download from Google Drive.",
                                            "Không tìm thấy file nào để tải từ Google Drive."
                                        );
                                    }
                                }));
                            }
                            catch
                            {
                                // Form có thể đã bị dispose lúc đang download, thì bỏ qua
                            }
                        }
                    }
                );

                // ===== 2. IMPORT CSV =====
                UpdateDownloadMessage(
                    "Importing gestures from CSV...",
                    "Đang import cử chỉ từ file CSV..."
                );

                int inserted = await _gestureDownloadService.ImportGesturesFromCsvAsync(userId);

                UpdateDownloadMessage(
                    "Updating request status.. .",
                    "Đang cập nhật trạng thái yêu cầu..."
                );

                await UpdateAllRequestsToSuccessfulAsync();
                await _gestureDownloadService.EnableGestureRequestAsync(userId);

                UpdateDownloadMessage(
                    $"Imported {inserted} gestures from CSV...",
                    $"Đã import {inserted} cử chỉ từ CSV..."
                );

                // ===== 3. REFRESH UI =====
                UpdateDownloadMessage(
                    "Refreshing gesture list...",
                    "Đang tải lại danh sách cử chỉ..."
                );

                await RefreshGesturesAsync();


                _canDownload = await _userService.CheckCanDownloadAsync(userId);
                _canRequest = await _userService.CheckCanRequestAsync(userId);
                // ===== 4. THÔNG BÁO =====
                CustomMessageBox.ShowSuccess(
                    I18nHelper.GetString(
                        $"Downloaded from Drive & imported {inserted} gestures.",
                        $"Đã tải từ Drive và import {inserted} cử chỉ."
                    ),
                    I18nHelper.GetString("Download gesture", "Tải cử chỉ")
                );
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError(ex.Message, "Download / Import error");
            }
            finally
            {
                HideDownloadLoading();

                await UpdateButtonStatesAsync();
                btnLanguageEN.Enabled = true;
                btnLanguageVN.Enabled = true;
                btnPresentation.Enabled = true;
                btnCustomGesture.Enabled = true;
                btnGestureControl.Enabled = true;
                btnHome.Enabled = true;
                btnInstruction.Enabled = true;
                btnProfile.Enabled = true;
                btnLogout.Enabled = true;
            }
        }
        /// <summary>
        /// Căn giữa panel loading và các phần tử bên trong
        /// </summary>
        private void CenterLoadingPanel()
        {
            if (panelLoading == null) return;

            panelLoading.Dock = DockStyle.Fill;
            panelLoading.BringToFront();

            // Căn lần đầu
            AlignLoadingControls();

            // Khi form resize thì căn lại
            panelLoading.Resize += (s, e) =>
            {
                AlignLoadingControls();
            };
        }
        private void AlignLoadingControls()
        {
            if (panelLoading == null || lblLoading == null || loadingSpinner == null)
                return;

            // Cho label tự co giãn theo text
            lblLoading.AutoSize = true;
            lblLoading.MaximumSize = new Size(panelLoading.Width - 80, 0); // chừa mép 2 bên 40px
            lblLoading.TextAlign = ContentAlignment.MiddleCenter;

            // Sau khi AutoSize, Width/Height sẽ đúng với text hiện tại
            lblLoading.Left = (panelLoading.Width - lblLoading.Width) / 2;
            lblLoading.Top = (panelLoading.Height - lblLoading.Height) / 2;

            // Spinner nằm phía trên label 20px
            loadingSpinner.Left = (panelLoading.Width - loadingSpinner.Width) / 2;
            loadingSpinner.Top = lblLoading.Top - loadingSpinner.Height - 20;
        }

        /// <summary>
        /// Update tất cả request đang ở trạng thái "Submit" sang "Successful"
        /// </summary>
        private async Task UpdateAllRequestsToSuccessfulAsync()
        {
            try
            {
                var requestService = new UserGestureRequestService();

                // Lấy tất cả config IDs
                List<string> configIds = new List<string>();

                if (isShowingUserGestures && userGestures != null)
                {
                    configIds = userGestures.Select(g => g.Id).ToList();
                }
                else if (defaultGestures != null)
                {
                    configIds = defaultGestures.Select(g => g.Id).ToList();
                }

                if (configIds.Count == 0)
                {
                    Console.WriteLine("[UpdateAllRequests] No configs found");
                    return;
                }

                // Lấy tất cả requests
                var requests = await requestService.GetLatestRequestsBatchAsync(userId, configIds);

                if (requests == null || requests.Count == 0)
                {
                    Console.WriteLine("[UpdateAllRequests] No requests found");
                    return;
                }

                int updatedCount = 0;

                // Update từng request sang Successful
                foreach (var request in requests)
                {
                    // Chỉ update nếu status là "Submit" (đang training)
                    if (request.Status != null)
                    {
                        string statusEn = request.Status.ContainsKey("en") ? request.Status["en"] : "";
                        string statusVi = request.Status.ContainsKey("vi") ? request.Status["vi"] : "";

                        if (statusEn == "Submit" || statusVi == "Gửi")
                        {
                            Console.WriteLine($"[UpdateAllRequests] Updating configId: {request.UserGestureConfigId}");

                            // ✅ GỌI METHOD ĐÃ CÓ SẴN
                            var success = await requestService.SetTrainingToSuccessfulAsync(
                                request.UserGestureConfigId,
                                userId
                            );

                            if (success)
                            {
                                updatedCount++;
                                Console.WriteLine($"[UpdateAllRequests] ✅ Updated: {request.UserGestureConfigId}");
                            }
                            else
                            {
                                Console.WriteLine($"[UpdateAllRequests] ❌ Failed: {request.UserGestureConfigId}");
                            }
                        }
                    }
                }

                Console.WriteLine($"[UpdateAllRequests] ✅ Updated {updatedCount}/{requests.Count} requests to Successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UpdateAllRequests] ❌ Error: {ex.Message}");
                // Không throw exception - download vẫn thành công
            }
        }
    }
}