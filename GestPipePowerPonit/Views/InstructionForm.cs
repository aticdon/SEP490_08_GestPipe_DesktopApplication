using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GestPipePowerPonit
{
    public partial class InstructionForm : Form
    {
        private const int BUTTON_SPACING = 40;
        private int buttonBottomMargin;
        private readonly ApiClient _apiClient;
        private readonly AuthService _authService;
        private readonly string _currentUserId = Properties.Settings.Default.UserId;
        Color fillActive1 = Color.Navy;         // FillColor
        Color fillActive2 = Color.DeepSkyBlue;  // FillColor2

        // Màu cho nút không được chọn
        Color fillNormal1 = Color.Black; // bạn tự chọn màu normal
        Color fillNormal2 = Color.Navy;


        // ✅ Không auto ép en-US nữa, chỉ là biến lưu trạng thái hiện tại
        private string _currentCultureCode = "en-US";


        private HomeUser _homeForm;
        private int _currentSubtab = 1;
        private Dictionary<(int, string), Image> _subtabImages;


        public InstructionForm(HomeUser homeForm)
        {
            InitializeComponent();


            btnGestureControl.Click += BtnGestureControl_Click;


            _apiClient = new ApiClient("https://localhost:7219");
            _authService = new AuthService();
            _homeForm = homeForm;


            // ✅ Load: chỉ apply từ CultureManager, không gọi API nữa
            this.Load += InstructionForm_Load;


            btnLanguageEN.Click += async (s, e) => await ChangeLanguageAsync("en-US");
            btnLanguageVN.Click += async (s, e) => await ChangeLanguageAsync("vi-VN");


            btnSubtab1.Click += btnSubtab1_Click;
            btnSubtab2.Click += btnSubtab2_Click;
            btnSubtab3.Click += btnSubtab3_Click;


            // ✅ Khi CultureManager đổi (từ form khác), form này cũng update theo
            CultureManager.CultureChanged += (s, e) =>
            {
                _currentCultureCode = CultureManager.CurrentCultureCode;
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(_currentCultureCode);


                ResourceHelper.SetCulture(_currentCultureCode, this);
                ApplyResourceToControls();
                ShowSubtabImage();
            };
        }

        private void SetActiveSubtab(int tab)
        {
            btnSubtab1.FillColor = (tab == 1) ? fillActive1 : fillNormal1;
            btnSubtab1.FillColor2 = (tab == 1) ? fillActive2 : fillNormal2;

            btnSubtab2.FillColor = (tab == 2) ? fillActive1 : fillNormal1;
            btnSubtab2.FillColor2 = (tab == 2) ? fillActive2 : fillNormal2;

            btnSubtab3.FillColor = (tab == 3) ? fillActive1 : fillNormal1;
            btnSubtab3.FillColor2 = (tab == 3) ? fillActive2 : fillNormal2;
        }
        // ✅ Chỉ đọc từ CultureManager, không fallback về en-US nếu API fail
        private void InstructionForm_Load(object sender, EventArgs e)
        {
            // Lấy culture hiện tại từ global I18n
            _currentCultureCode = CultureManager.CurrentCultureCode;


            // Apply culture vào thread + form
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(_currentCultureCode);
            ResourceHelper.SetCulture(_currentCultureCode, this);
            ApplyResourceToControls();
            ShowSubtabImage();


            // Đảm bảo highlight button đầu tiên và load nội dung tab 1
            btnSubtab1_Click(btnSubtab1, EventArgs.Empty);
        }


        private void BtnGestureControl_Click(object sender, EventArgs e)
        {
            ListDefaultGestureForm defaultGesture = new ListDefaultGestureForm(_homeForm);
            defaultGesture.Show();
            this.Hide();
        }


        // 🔹 Subtab 1
        private void btnSubtab1_Click(object sender, EventArgs e)
        {
            _currentSubtab = 1;
            SetActiveSubtab(1);
            ShowSubtabImage();
        }


        // 🔹 Subtab 2
        private void btnSubtab2_Click(object sender, EventArgs e)
        {
            _currentSubtab = 2;
            SetActiveSubtab(2);
            ShowSubtabImage();
        }


        // 🔹 Subtab 3
        private void btnSubtab3_Click(object sender, EventArgs e)
        {
            _currentSubtab = 3;
            SetActiveSubtab(3);
            ShowSubtabImage();
        }


        //private void ShowSubtabImage()
        //{
        //    Image img = null;
        //    switch (_currentSubtab)
        //    {
        //        case 1: img = Properties.Resources.Instruction_Tab1; break;
        //        case 2: img = Properties.Resources.Instruction_Tab2; break;
        //        case 3: img = Properties.Resources.Instruction_Tab3; break;
        //    }
        //    guna2PictureBox1.Image = img;
        //}
        private void ShowSubtabImage()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(_currentCultureCode); // <-- Luôn cưỡng chế lại trước khi lấy ảnh!
            string resourceName = _currentSubtab switch
            {
                1 => "Instruction_Tab1",
                2 => "Instruction_Tab2",
                3 => "Instruction_Tab3",
                _ => "Instruction_Tab1"
            };

            Console.WriteLine($"Tab: {_currentSubtab}, Culture: {Thread.CurrentThread.CurrentUICulture.Name}, ResourceName: {resourceName}");

            var imgObj = Properties.Resources.ResourceManager.GetObject(resourceName, Thread.CurrentThread.CurrentUICulture);
            if (imgObj is Image img)
                guna2PictureBox1.Image = img;
            else
                guna2PictureBox1.Image = null;
        }
        private async Task ChangeLanguageAsync(string cultureCode)
        {
            _currentCultureCode = cultureCode;
            CultureManager.CurrentCultureCode = _currentCultureCode;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
            ResourceHelper.SetCulture(_currentCultureCode, this);
            ApplyResourceToControls();
            ShowSubtabImage();
            await _apiClient.SetUserLanguageAsync(_currentUserId, cultureCode);
        }


        private void ApplyResourceToControls()
        {
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;


            btnSubtab1.Text = I18nHelper.GetString(
                "Gesture Recording Guide",
                "Hướng dẫn ghi nhận động tác"
            );
            btnSubtab2.Text = I18nHelper.GetString(
                "3D Model Setup",
                "Thiết lập lỗi mô hình 3D"
            );
            btnSubtab3.Text = I18nHelper.GetString(
                "Environment & Practice Tips",
                "Mẹo môi trường & luyện tập"
            );
            // Nếu muốn text cho tab 3:
            // btnSubtab3.Text = I18nHelper.GetString("Presentation Environment", "Môi trường trình bày");
        }


        private void btnPresentation_Click(object sender, EventArgs e)
        {
            PresentationForm form1 = new PresentationForm(_homeForm);
            form1.Show();
            this.Hide();
        }


        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            ListRequestGestureForm usergestureForm = new ListRequestGestureForm(_homeForm);
            usergestureForm.Show();
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
                Console.WriteLine("[InstructionForm] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));


                var response = await _authService.LogoutAsync();


                if (response?.Success == true)
                {
                    Console.WriteLine("[InstructionForm] ✅ Logout successful");


                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );


                    var loginForm = new LoginForm();
                    this.Hide();
                    loginForm.Show();
                    this.Dispose();


                    Console.WriteLine("[InstructionForm] ✅ Returned to LoginForm");
                    Console.WriteLine(new string('=', 60) + "\n");
                }
                else
                {
                    Console.WriteLine($"[InstructionForm] ❌ Logout failed: {response?.Message}");


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
                Console.WriteLine($"[InstructionForm] ❌ Exception: {ex.Message}");


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


        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                ProfileForm profileForm = new ProfileForm(_currentUserId, _homeForm);
                this.Hide();
                profileForm.Show();
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

        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Hide();
        }
    }
}