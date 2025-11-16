using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
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
        //private UserGestureConfigService _gestureService = new UserGestureConfigService();
        private DefaultGestureService _gestureService = new DefaultGestureService();
        private HomeUser _homeForm;
        //private List<UserGestureConfigDto> gestures;
        private List<DefaultGestureDto> gestures;
        private string userId = Properties.Settings.Default.UserId;
        private readonly ApiClient _apiClient;
        private UserService _userService = new UserService();
        private ProfileService _profileService = new ProfileService();

        private bool _canRequest;
        //public FormUserGesture(HomeUser homeForm, string userId)
        public ListRequestGestureForm(HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += ListRequestGestureForm_Load;
            _homeForm = homeForm;
            guna2DataGridView1.CellContentClick += guna2DataGridView1_CellContentClick;
            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");
            _apiClient = new ApiClient("https://localhost:7219");
            CultureManager.CultureChanged += async (s, e) =>
            {
                ApplyLanguage(CultureManager.CurrentCultureCode);
                await LoadDefaultGesturesAsync();
            };
        }

        private async void ListRequestGestureForm_Load(object sender, EventArgs e)
        {
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);

            //var userService = new UserService();
            _canRequest = await _userService.CheckCanRequestAsync(userId);

            btnRequest.Enabled = _canRequest;                
            btnRequest.ForeColor = _canRequest ? Color.White : Color.Black;
            await LoadDefaultGesturesAsync();
        }
        //private async Task LoadDefaultGesturesAsync()
        //{
        //    try
        //    {
        //        //gestures = await _gestureService.GetUserGesturesAsync(userId);
        //        gestures = await _gestureService.GetDefaultGesturesAsync();
        //        guna2DataGridView1.Rows.Clear();
        //        guna2DataGridView1.AllowUserToAddRows = false;

        //        var requestService = new UserGestureRequestService();

        //        foreach (var config in gestures)
        //        {
        //            // GỌI HÀM SERVICE lấy request mới nhất dựa vào config.Id
        //            var request = await requestService.GetLatestRequestByConfigAsync(config.Id);
        //            string statusToShow, timeToShow;
        //            string accuracToShow;
        //            object viewIcon, customIcon;
        //            if(!_canRequest)
        //            {
        //                statusToShow = I18nHelper.GetLocalized(config.Status);
        //                timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = $"{config.Accuracy * 100:F1}%";
        //                viewIcon = Properties.Resources.eye_gray;
        //                customIcon = Properties.Resources.CustomCameraGray;
        //            }
        //            else
        //            {
        //                if (request != null)
        //                {
        //                    statusToShow = I18nHelper.GetLocalized(request.Status);
        //                    // Nếu có trường CreatedAt thì sử dụng, nếu không cần sửa DTO/model cho đủ.
        //                    timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
        //                    accuracToShow = "N/A";
        //                    viewIcon = Properties.Resources.eye_gray;
        //                    customIcon = Properties.Resources.CustomCameraGray;
        //                }
        //                else
        //                {
        //                    statusToShow = I18nHelper.GetLocalized(config.Status);
        //                    timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
        //                    accuracToShow = $"{config.Accuracy * 100:F1}%";
        //                    viewIcon = Properties.Resources.eye;
        //                    customIcon = Properties.Resources.CustomCamera;
        //                }
        //            }

        //            guna2DataGridView1.Rows.Add(
        //                I18nHelper.GetLocalized(config.Name),
        //                I18nHelper.GetLocalized(config.Type),
        //                accuracToShow,
        //                statusToShow,
        //                timeToShow,
        //                viewIcon,
        //                customIcon
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Không thể tải danh sách gesture!\n" + ex.Message);
        //    }
        //}

        //private async Task LoadDefaultGesturesAsync()
        //{
        //    try
        //    {
        //        panelLoading.Visible = true;
        //        lblLoading.Text = Properties.Resources.ProfileForm_Loading;

        //        gestures = await _gestureService.GetDefaultGesturesAsync();
        //        guna2DataGridView1.Rows.Clear();
        //        guna2DataGridView1.AllowUserToAddRows = false;

        //        var requestService = new UserGestureRequestService();

        //        foreach (var config in gestures)
        //        {
        //            var request = await requestService.GetLatestRequestByConfigAsync(config.Id);

        //            string statusToShow, timeToShow, accuracToShow;
        //            object viewIcon, customIcon;

        //            if (!_canRequest)
        //            {
        //                // Toàn bộ icon đều màu xám
        //                statusToShow = I18nHelper.GetLocalized(config.Status);
        //                timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = $"{config.Accuracy * 100:F1}%";
        //                viewIcon = Properties.Resources.eye_gray;
        //                customIcon = Properties.Resources.CustomCameraGray;
        //            }
        //            else if (request != null)
        //            {
        //                statusToShow = I18nHelper.GetLocalized(request.Status);
        //                timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = "N/A";
        //                viewIcon = Properties.Resources.eye_gray;
        //                customIcon = Properties.Resources.CustomCameraGray;
        //            }
        //            else
        //            {
        //                statusToShow = I18nHelper.GetLocalized(config.Status);
        //                timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
        //                accuracToShow = $"{config.Accuracy * 100:F1}%";
        //                viewIcon = Properties.Resources.eye;
        //                customIcon = Properties.Resources.CustomCamera;
        //            }

        //            guna2DataGridView1.Rows.Add(
        //                I18nHelper.GetLocalized(config.Name),
        //                I18nHelper.GetLocalized(config.Type),
        //                accuracToShow,
        //                statusToShow,
        //                timeToShow,
        //                viewIcon,
        //                customIcon
        //            );
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Không thể tải danh sách gesture!\n" + ex.Message);
        //    }
        //    finally
        //    {
        //        panelLoading.Visible = false;
        //    }
        //}
        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                panelLoading.Visible = true;
                panelLoading.BringToFront();
                lblLoading.Text = Properties.Resources.List_Loading;

                gestures = await _gestureService.GetDefaultGesturesAsync();
                guna2DataGridView1.Rows.Clear();
                guna2DataGridView1.AllowUserToAddRows = false;

                var requestService = new UserGestureRequestService();

                // Song song lấy requests cho tốc độ nhanh
                var requestTasks = gestures.Select(config => requestService.GetLatestRequestByConfigAsync(config.Id)).ToList();
                var requests = await Task.WhenAll(requestTasks);

                var rowsToAdd = new List<object[]>();
                for (int i = 0; i < gestures.Count; i++)
                {
                    var config = gestures[i];
                    var request = requests[i];

                    string statusToShow, timeToShow, accuracToShow;
                    object viewIcon, customIcon;

                    if (!_canRequest)
                    {
                        statusToShow = I18nHelper.GetLocalized(config.Status);
                        timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = $"{config.Accuracy * 100:F1}%";
                        viewIcon = Properties.Resources.eye_gray;
                        customIcon = Properties.Resources.CustomCameraGray;
                    }
                    else if (request != null)
                    {
                        statusToShow = I18nHelper.GetLocalized(request.Status);
                        timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = "N/A";
                        viewIcon = Properties.Resources.eye_gray;
                        customIcon = Properties.Resources.CustomCameraGray;
                    }
                    else
                    {
                        statusToShow = I18nHelper.GetLocalized(config.Status);
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
                foreach (var row in rowsToAdd)
                {
                    guna2DataGridView1.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải danh sách gesture!\n" + ex.Message);
            }
            finally
            {
                panelLoading.Visible = false;
            }
        }
        private async void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var basic = gestures[e.RowIndex];

            // LẤY request cho row hiện tại
            var requestService = new UserGestureRequestService();
            var request = await requestService.GetLatestRequestByConfigAsync(basic.Id);

            // Nếu là dòng có request thì KHÔNG xử lý View/Custom
            if (request != null &&
                (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index ||
                 e.ColumnIndex == guna2DataGridView1.Columns["ColumnCustom"].Index))
            {
                return;
            }

            // Nếu không có request thì cho phép thao tác như cũ:
            if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnView"].Index)
            {
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);

                if (detail == null)
                {
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
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnCustom"].Index)
            {
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                string poseLabel = detail.PoseLabel;
                string userGesture = I18nHelper.GetLocalized(detail.Name);
                //string userName = Properties.Settings.Default.UserId;
                string userName = "unknown";
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
                    // Fallback: Có thể lấy từ UserService nếu có method GetUser
                    userName = $"User {userId.Substring(0, Math.Min(8, userId.Length))}";
                }
                string gestureId = basic.Id;
                var customForm = new CustomGestureForm(_homeForm, gestureId, userName, poseLabel, userGesture);
                customForm.Show();
                this.Hide();
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
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            btnRequest.Text = Properties.Resources.Btn_RequestGesture;
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
            var requestForm = new RequestGestureForm(userId);
            requestForm.ShowDialog();
        }
    }
}