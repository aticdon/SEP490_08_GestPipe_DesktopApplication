using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using System;
using System.Drawing;
using System.Linq;
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
        private string _currentCultureCode = "";
        private ProfileService _profileService = new ProfileService();
        private string userName = "";

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
            Console.WriteLine("=== HomeForm Font & DPI Debug ===");
            Console.WriteLine($"Form Font: {this.Font.Name} {this.Font.Size}F");
            Console.WriteLine($"AutoScaleMode: {this.AutoScaleMode}");
            Console.WriteLine($"AutoScaleDimensions: {this.AutoScaleDimensions}");
            Console.WriteLine($"Form Size: {this.Size}");
            Console.WriteLine("==================================");
            var currentScale = this.CurrentAutoScaleDimensions;
            var designScale = new SizeF(8F, 16F);
            var profileResponse = await _profileService.GetProfileAsync(_currentUserId);
            userName = profileResponse.Data.Profile.FullName;
            string WebcomeMessage = $"WellCome {userName}";
            lblWelcome.Text = WebcomeMessage;
            Console.WriteLine($"Design: {designScale}\nCurrent: {currentScale}\nScale Factor: {currentScale.Width / designScale.Width}");
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
            PresentationForm form1 = new PresentationForm(this);
            form1.Show();
            this.Hide();
        }

        private void BtnGestureControl_Click(object sender, EventArgs e)
        {
            ListDefaultGestureForm defaultGesture = new ListDefaultGestureForm(this);
            defaultGesture.Show();
            this.Hide();
        }

        private async Task LoadUserAndApplyLanguageAsync()
        {
            try
            {
                var user = await _apiClient.GetUserAsync(_currentUserId);
                _currentCultureCode = (user != null && !string.IsNullOrWhiteSpace(user.UiLanguage)) ? user.UiLanguage : "en-US";
                CultureManager.CurrentCultureCode = _currentCultureCode;
                ResourceHelper.SetCulture(_currentCultureCode, this);
                ApplyResourceToControls();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Không thể load user: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private async Task ChangeLanguageAsync(string cultureCode)
        {
            try
            {
                _currentCultureCode = cultureCode;
                CultureManager.CurrentCultureCode = _currentCultureCode;
                ResourceHelper.SetCulture(_currentCultureCode, this);
                ApplyResourceToControls();

                // ✅ API call SAU
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

        private void ApplyResourceToControls()
        {
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnPresent.Text = Properties.Resources.Btn_Present;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnTraining.Text = Properties.Resources.Btn_Training;
            //lblWelcome.Text = Properties.Resources.Home_Welcome;
            string WelcomeMessage = $"WellCome {userName}";
            string WelcomeMessageVN = $"Chào mừng {userName}";
            lblWelcome.Text = I18nHelper.GetString(WelcomeMessage,WelcomeMessageVN);
            lblSubtitle.Text = Properties.Resources.LblSubtitle;
            btnPresentation.Text = Properties.Resources.Btn_Present;
        }

        //private void btnTrainingGesture_Click(object sender, EventArgs e)
        //{
        //    FormUserGesture usergestureForm = new FormUserGesture(this);
        //    usergestureForm.Show();
        //    this.Hide();
        //}

        private void btnPresentation_Click(object sender, EventArgs e)
        {
            PresentationForm form1 = new PresentationForm(this);
            form1.Show();
            this.Hide();
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
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
                Console.WriteLine("[HomeUser] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[HomeUser] ✅ Logout successful");

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    // ✅ FIX: Không set FormClosed event
                    var loginForm = new LoginForm();

                    // ✅ Close HomeUser TRƯỚC
                    this.Hide();

                    // ✅ Show LoginForm
                    loginForm.Show();

                    // ✅ Dispose HomeUser sau khi LoginForm hiển thị
                    this.Dispose();

                    Console.WriteLine("[HomeUser] ✅ Returned to LoginForm");
                    Console.WriteLine(new string('=', 60) + "\n");
                }
                else
                {
                    Console.WriteLine($"[HomeUser] ❌ Logout failed: {response?.Message}");

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
                Console.WriteLine($"[HomeUser] ❌ Exception: {ex.Message}");

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

        // ✅ PROFILE BUTTON IMPLEMENTATION
        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileForm profileForm = new ProfileForm(_currentUserId, this);

                this.Hide();

                profileForm.Show();

                //profileForm.FormClosed += (s, args) =>
                //{
                //    this.Show();
                //};
            }
            catch (Exception ex)
            {
                string errorMessage = _currentCultureCode == "vi-VN"
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
            ListRequestGestureForm usergestureForm = new ListRequestGestureForm(this);
            usergestureForm.Show();
            this.Hide();
        }

        private void btnInstruction_Click(object sender, EventArgs e)
        {
            InstructionForm instructionForm = new InstructionForm(this);
            instructionForm.Show();
            this.Hide();
        }

        private void btnTraining_Click(object sender, EventArgs e)
        {
            ListDefaultGestureForm defaultGesture = new ListDefaultGestureForm(this);
            defaultGesture.Show();
            this.Hide();
        }
    }
}