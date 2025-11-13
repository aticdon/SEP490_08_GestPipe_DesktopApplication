using GestPipePowerPonit.I18n;
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
            //btnTraining.Click += BtnTraining_Click;
            btnGestureControl.Click += BtnGestureControl_Click;
            _currentUserId = currentUserId;
            _apiClient = new ApiClient("https://localhost:7219");

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
            // Lấy chiều rộng và chiều cao hiện tại của Panel chứa
            int panelWidth = pnlMain.ClientSize.Width;
            int panelHeight = pnlMain.ClientSize.Height;

            // Tính tổng chiều rộng của 2 nút và khoảng cách
            int totalWidth = btnTraining.Width + btnPresent.Width + BUTTON_SPACING;

            // Tính vị trí X của nút đầu tiên (căn giữa)
            int startX = (panelWidth - totalWidth) / 2;

            // Tính vị trí Y (giữ nguyên khoảng cách so với đáy panel)
            int buttonY = panelHeight - btnTraining.Height - buttonBottomMargin;

            // Đặt lại vị trí cho nút Training
            btnTraining.Location = new Point(startX, buttonY);

            // Đặt lại vị trí cho nút Present
            btnPresent.Location = new Point(startX + btnTraining.Width + BUTTON_SPACING, buttonY);
        }

        private void guna2Panel2_Resize(object sender, EventArgs e)
        {
            // Gọi phương thức căn giữa mỗi khi kích thước Panel thay đổi
            CenterButtons();
        }

        private void BtnPresent_Click(object sender, EventArgs e)
        {
            // Mở Form1 để present
            Form1 form1 = new Form1(this);
            //form1.ApplyLanguage(_currentCultureCode); // Gọi hàm đổi ngôn ngữ cho Form1
            form1.Show();
            this.Hide();
        }

        //private void BtnTraining_Click(object sender, EventArgs e)
        //{
        //    FormCustomGesture customForm = new FormCustomGesture(this);
        //    customForm.Show();
        //    this.Hide();
        //    MessageBox.Show("Training feature coming soon!");
        //}

        private void BtnGestureControl_Click(object sender, EventArgs e)
        {
            // Mở FormCustomGesture
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
                _currentCultureCode = cultureCode; // Cập nhật biến khi đổi
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
            // ... thêm các label/button khác nếu có
        }

        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            //FormUserGesture usergestureForm = new FormUserGesture(this, _currentUserId);
            FormUserGesture usergestureForm = new FormUserGesture(this);
            usergestureForm.Show();
            this.Hide();
        }

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(this);
            //form1.ApplyLanguage(_currentCultureCode); // Gọi hàm đổi ngôn ngữ cho Form1
            form1.Show();
            this.Hide();
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}