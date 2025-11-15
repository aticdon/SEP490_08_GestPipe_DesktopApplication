using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace GestPipePowerPonit
{
    public partial class FormUserGestureCustom : Form
    {
        private UserGestureConfigService _gestureService = new UserGestureConfigService();
        private HomeUser _homeForm;
        private List<UserGestureConfigDto> gestures;
        private string userId = Properties.Settings.Default.UserId;
        private readonly ApiClient _apiClient;
        private UserService _userService = new UserService();

        //public FormUserGesture(HomeUser homeForm, string userId)
        public FormUserGestureCustom(HomeUser homeForm)
        {
            InitializeComponent();
            this.Load += FormUserGestureCustom_Load;
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

        private async void FormUserGestureCustom_Load(object sender, EventArgs e)
        {
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            await LoadDefaultGesturesAsync();

            //var userService = new UserService();
            bool canRequest = await _userService.CheckCanRequestAsync(userId);

            btnRequest.Enabled = canRequest;                // Nếu không được, sẽ bị disable
            btnRequest.ForeColor = canRequest ? Color.Black : Color.Gray; // Làm mờ text nếu cần
        }
        private async Task LoadDefaultGesturesAsync()
        {
            try
            {
                gestures = await _gestureService.GetUserGesturesAsync(userId);
                guna2DataGridView1.Rows.Clear();
                guna2DataGridView1.AllowUserToAddRows = false;

                var requestService = new UserGestureRequestService();

                foreach (var config in gestures)
                {
                    // GỌI HÀM SERVICE lấy request mới nhất dựa vào config.Id
                    var request = await requestService.GetLatestRequestByConfigAsync(config.Id);
                    string statusToShow, timeToShow;
                    string accuracToShow;
                    if (request != null)
                    {
                        statusToShow = I18nHelper.GetLocalized(request.Status);
                        // Nếu có trường CreatedAt thì sử dụng, nếu không cần sửa DTO/model cho đủ.
                        timeToShow = request.CreatedAt.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = "N/A";
                    }
                    else
                    {
                        statusToShow = I18nHelper.GetLocalized(config.Status);
                        timeToShow = config.LastUpdate.ToString("dd-MM-yyyy HH:mm");
                        accuracToShow = $"{config.Accuracy * 100:F1}%";
                    }
                    guna2DataGridView1.Rows.Add(
                        I18nHelper.GetLocalized(config.Name),
                        I18nHelper.GetLocalized(config.Type),
                        accuracToShow,
                        statusToShow,
                        timeToShow,
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
            else if (e.ColumnIndex == guna2DataGridView1.Columns["ColumnCustom"].Index)
            {
                var detail = await _gestureService.GetGestureDetailAsync(basic.Id);
                string poseLabel = detail.PoseLabel;
                string userGesture = I18nHelper.GetLocalized(detail.Name);
                string userName = Properties.Settings.Default.UserId;
                string gestureId = basic.Id;
                var customForm = new FormCustomGesture(_homeForm, gestureId, userName, poseLabel, userGesture);
                customForm.Show();
                this.Hide();
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

        private void btnTrainingGesture_Click(object sender, EventArgs e)
        {
            FormUserGesture uGestureForm = new FormUserGesture(_homeForm);
            uGestureForm.Show();
            this.Hide();
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            var requestForm = new FormRequestGestures(userId);
            requestForm.ShowDialog();
        }
    }
}