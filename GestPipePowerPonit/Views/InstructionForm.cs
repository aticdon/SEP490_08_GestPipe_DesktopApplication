using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Linq;
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
        private string _currentCultureCode = "";
        private HomeUser _homeForm;
        private int _currentSubtab = 1; // Thêm biến theo dõi tab hiện tại

        public InstructionForm(HomeUser homeForm)
        {
            InitializeComponent();

            btnGestureControl.Click += BtnGestureControl_Click;

            _apiClient = new ApiClient("https://localhost:7219");
            _authService = new AuthService();
            _homeForm = homeForm;
            this.Load += InstructionForm_Load;
            btnLanguageEN.Click += async (s, e) => await ChangeLanguageAsync("en-US");
            btnLanguageVN.Click += async (s, e) => await ChangeLanguageAsync("vi-VN");
            btnSubtab1.Click += BtnSubtab1_Click;
            btnSubtab2.Click += BtnSubtab2_Click;


            CultureManager.CultureChanged += (s, e) =>
            {
                ResourceHelper.SetCulture(CultureManager.CurrentCultureCode, this);
                ApplyResourceToControls();
                // Tải lại nội dung tab khi ngôn ngữ thay đổi
                LoadSubtabContent(_currentSubtab);
            };
        }

        private async void InstructionForm_Load(object sender, EventArgs e)
        {
            await LoadUserAndApplyLanguageAsync();

            // Đảm bảo highlight button đầu tiên và load nội dung tab 1
            BtnSubtab1_Click(btnSubtab1, EventArgs.Empty);
        }

        private void BtnGestureControl_Click(object sender, EventArgs e)
        {
            ListDefaultGestureForm defaultGesture = new ListDefaultGestureForm(_homeForm);
            defaultGesture.Show();
            this.Hide();
        }

        // Fix 1: Thêm logic Highlight và gọi LoadSubtabContent_Tab1
        private void BtnSubtab1_Click(object sender, EventArgs e)
        {
            _currentSubtab = 1;
            LoadSubtabContent_Tab1(CultureManager.CurrentCultureCode);
            btnSubtab1.FillColor = System.Drawing.Color.FromArgb(25, 25, 25); // Highlight active
            btnSubtab2.FillColor = System.Drawing.Color.FromArgb(18, 125, 202);
        }

        // Fix 1: Thêm logic Highlight và gọi LoadSubtabContent_Tab2
        private void BtnSubtab2_Click(object sender, EventArgs e)
        {
            _currentSubtab = 2;
            LoadSubtabContent_Tab2(CultureManager.CurrentCultureCode);
            btnSubtab2.FillColor = System.Drawing.Color.FromArgb(25, 25, 25); // Highlight active
            btnSubtab1.FillColor = System.Drawing.Color.FromArgb(18, 125, 202);
        }

        // Hàm cũ không cần thiết, đã bị xóa khỏi code này
        // private void LoadSubtabContent(int subtabNumber) { ... }

        // Hàm này được giữ lại chỉ để gọi đúng hàm Load_Tab
        private void LoadSubtabContent(int subtabNumber)
        {
            if (subtabNumber == 1)
            {
                LoadSubtabContent_Tab1(CultureManager.CurrentCultureCode);
            }
            else
            {
                LoadSubtabContent_Tab2(CultureManager.CurrentCultureCode);
            }
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

                await _apiClient.SetUserLanguageAsync(_currentUserId, cultureCode);

                CustomMessageBox.ShowSuccess(
                    Properties.Resources.Message_ChangeLanguageSuccess,
                    Properties.Resources.Title_Success
                );
            }
            catch (Exception)
            {
                CustomMessageBox.ShowError(
                    Properties.Resources.Message_ChangeLanguageFailed,
                    Properties.Resources.Title_Error
                );
            }
        }

        private void ApplyResourceToControls()
        {
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;

            btnSubtab1.Text = I18nHelper.GetString("Interact with 3D Model", "Tương tác với Mô hình 3D");
            btnSubtab2.Text = I18nHelper.GetString("Camera & Lighting", "Camera & Lighting");
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
            // ... [Logout logic]
            // Giữ nguyên logic logout
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

                    var loginForm = new LoginForm();
                    this.Hide();
                    loginForm.Show();
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

        // =========================================================================
        // START: CONTENT LOADING LOGIC (Sửa lỗi Form và Font)
        // =========================================================================

        // FIX 3 & 4: Sửa lỗi căn chỉnh và áp dụng Font Size cho Tab 1
        private void LoadSubtabContent_Tab1(string cultureCode)
        {
            pnlContentHolder.Controls.Clear();

            // 💥 Tiêu đề Chính (16pt, Bold)

            // Content Panel (Start point relative to pnlContentHolder)
            var contentPanel = new Guna2Panel
            {
                Size = new Size(pnlContentHolder.Width - 50, pnlContentHolder.Height - 50),
                Location = new Point(25, 25), // Đặt nội dung cách lề 25px
                FillColor = Color.Transparent,
                Parent = pnlContentHolder
            };

            // --- LEFT COLUMN: RULES & FIXES ---
            // 💥 Tiêu đề Phụ 1: QUY TẮC & XỬ LÝ SỰ CỐ (13pt, Bold)
            var headerRules = CreateLabel(I18nHelper.GetString("Prerequisites & Troubleshooting", "QUY TẮC & XỬ LÝ SỰ CỐ"), new Point(0, 0), true, 13F);
            contentPanel.Controls.Add(headerRules);

            // Rule 1: Tên file (11pt, Regular)
            var rule1 = CreateIconText(I18nHelper.GetString("File name matches Slide Title", "Tên file 3D phải trùng với Slide Title"), Color.LightGreen, new Point(0, 40));
            contentPanel.Controls.Add(rule1);

            // Rule 2: Cùng thư mục (11pt, Regular)
            var rule2 = CreateIconText(I18nHelper.GetString("File in same folder as PPTX", "File nằm cùng thư mục với PPTX"), Color.LightGreen, new Point(0, 75));
            contentPanel.Controls.Add(rule2);

            // 💥 Tiêu đề Phụ 2: LỖI THƯỜNG GẶP (13pt, Bold)
            var headerError = CreateLabel(I18nHelper.GetString("Common Errors & Fixes:", "LỖI THƯỜNG GẶP & CÁCH XỬ LÝ:"), new Point(0, 140), true, 13F);
            contentPanel.Controls.Add(headerError);

            // Lỗi & Fix (11pt, Regular, Icon Đỏ)
            var errorFix = CreateIconText(I18nHelper.GetString("Model not displayed ➡ Check name, format (.glb/.obj), folder.", "File 3D không hiển thị ➡ Kiểm tra tên, format (.glb/.obj), thư mục."), Color.Red, new Point(0, 180));
            contentPanel.Controls.Add(errorFix);

            // --- RIGHT COLUMN: EXAMPLE STRUCTURE ---
            // 💥 Tiêu đề Phụ 3: VÍ DỤ CẤU TRÚC THƯ MỤC (13pt, Bold)
            var headerExample = CreateLabel(I18nHelper.GetString("Example Folder Structure:", "VÍ DỤ CẤU TRÚC THƯ MỤC:"), new Point(400, 0), true, 13F);
            contentPanel.Controls.Add(headerExample);

            // Nội dung Ví dụ (11pt, Regular, LightGray)
            var exampleText = I18nHelper.GetString(
                "/PresentationFolder\n├── MyPresentation.pptx\n├── heart.glb\n└── engine.obj",
                "/PresentationFolder\n├── MyPresentation.pptx\n├── heart.glb\n└── engine.obj"
            );

            // Sử dụng FontStyle.Regular cho văn bản code/structure
            var exampleLabel = CreateLabel(exampleText, new Point(400, 40), false, 11F, FontStyle.Regular, Color.LightGray);
            contentPanel.Controls.Add(exampleLabel);
        }

        // FIX 3: Áp dụng Font Size cho Tab 2
        private void LoadSubtabContent_Tab2(string cultureCode)
        {
            pnlContentHolder.Controls.Clear();

            // Content Panel
            var contentPanel = new Guna2Panel
            {
                Size = new Size(pnlContentHolder.Width - 50, pnlContentHolder.Height - 50),
                Location = new Point(25, 25), // Đặt nội dung cách lề 25px
                FillColor = Color.Transparent,
                Parent = pnlContentHolder
            };

            int y = 0;

            // Item 1: Distance (11pt Regular)
            var num1 = CreateLabel("1.", new Point(0, y), true, 11F, FontStyle.Bold, Color.LightBlue);
            var text1 = CreateLabel(I18nHelper.GetString("Distance with camera: 0.8-1.5m (recommended 1.2m)", "Khoảng cách với camera: 0.8-1.5m (khuyến khích 1.2m)"), new Point(30, y), false, 11F);
            contentPanel.Controls.Add(num1);
            contentPanel.Controls.Add(text1);
            y += 40;

            // Item 2: Lighting (11pt Regular)
            var num2 = CreateLabel("2.", new Point(0, y), true, 11F, FontStyle.Bold, Color.LightBlue);
            var text2 = CreateLabel(I18nHelper.GetString("Lighting Conditions: Good lighting, even, no shadows", "Ánh sáng (Lighting Conditions): Ánh sáng tốt, sáng đều, không bóng đổ"), new Point(30, y), false, 11F);
            contentPanel.Controls.Add(num2);
            contentPanel.Controls.Add(text2);
            y += 40;

            // Item 3: Environment (11pt Regular)
            var num3 = CreateLabel("3.", new Point(0, y), true, 11F, FontStyle.Bold, Color.LightBlue);
            var text3 = CreateLabel(I18nHelper.GetString("Presentation Environment: Avoid complex backgrounds or movement", "Môi trường trình bày: Tránh background phức tạp hoặc có chuyển động"), new Point(30, y), false, 11F);
            contentPanel.Controls.Add(num3);
            contentPanel.Controls.Add(text3);
            y += 40;

            // Item 4: Practice (11pt Regular)
            var num4 = CreateLabel("4.", new Point(0, y), true, 11F, FontStyle.Bold, Color.LightBlue);
            var text4 = CreateLabel(I18nHelper.GetString("Practice gestures before presentation", "Luyện gesture trước buổi trình bày"), new Point(30, y), false, 11F);
            contentPanel.Controls.Add(num4);
            contentPanel.Controls.Add(text4);
            y += 60; // Extra spacing

            // Item 5: Notes for Custom Gestures (Sub-section) (11pt Regular)
            var num5 = CreateLabel("5.", new Point(0, y), true, 11F, FontStyle.Bold, Color.LightBlue);
            // Tiêu đề sub-section được in đậm và có màu trắng
            //                             text                                         location       isBold   fontSize   color (tham số thứ 6)
            //var text5Header = CreateLabel(I18nHelper.GetString("Notes for Custom Gestures:", "Lưu ý khi tạo Custom Gesture:"), new Point(30, y), true, 11F);
            var text5Header = CreateLabel(I18nHelper.GetString("Notes for Custom Gestures:", "Lưu ý khi tạo Custom Gesture:"), new Point(30, y), true, 11F, FontStyle.Regular, Color.White);
            contentPanel.Controls.Add(num5);
            contentPanel.Controls.Add(text5Header);
            y += 30;

            // Nội dung chi tiết (11pt Regular, LightGray)
            var note1 = CreateLabel(I18nHelper.GetString("- Avoid duplicates", "- Không trùng với cử chỉ khác"), new Point(50, y), false, 11F, FontStyle.Regular, Color.LightGray);
            contentPanel.Controls.Add(note1);
            y += 30;

            var note2 = CreateLabel(I18nHelper.GetString("- Record 5 times", "- Ghi 5 lần"), new Point(50, y), false, 11F, FontStyle.Regular, Color.LightGray);
            contentPanel.Controls.Add(note2);
            y += 30;

            var note3 = CreateLabel(I18nHelper.GetString("- Keep consistent", "- Giữ ổn định"), new Point(50, y), false, 11F, FontStyle.Regular, Color.LightGray);
            contentPanel.Controls.Add(note3);
            y += 30;
        }

        // =========================================================================
        // END: CONTENT LOADING LOGIC
        // =========================================================================

        // FIX 3: Cập nhật hàm helper CreateLabel để nhận fontSize
        private System.Windows.Forms.Label CreateLabel(string text, Point location, bool isBold, float fontSize = 11F, FontStyle style = FontStyle.Regular, Color color = default)
        {
            if (color == default) color = Color.White;
            var label = new System.Windows.Forms.Label();
            label.Text = text.Trim();
            label.ForeColor = color;
            label.BackColor = Color.Transparent;
            // Cập nhật Font để sử dụng fontSize
            label.Font = new Font("Segoe UI", fontSize, isBold ? FontStyle.Bold : style);
            label.AutoSize = true;
            label.Location = location;
            return label;
        }

        // FIX 3: Cập nhật hàm helper CreateIconText để gọi CreateLabel với fontSize chính xác (11F)
        private Guna2Panel CreateIconText(string text, Color iconColor, Point location)
        {
            var panel = new Guna2Panel
            {
                Size = new Size(pnlContentHolder.Width - 50, 30), // Fit to content area
                Location = location,
                FillColor = Color.Transparent,
                Height = 30
            };

            var icon = new System.Windows.Forms.PictureBox
            {
                Image = (iconColor == Color.LightGreen) ? Properties.Resources.icon_check : Properties.Resources.icon_error,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(20, 20),
                Location = new Point(0, 0)
            };
            panel.Controls.Add(icon);

            // Gọi CreateLabel với fontSize mặc định 11F
            var label = CreateLabel(text, new Point(30, 0), false, 11F);
            panel.Controls.Add(label);

            panel.Height = label.Height;

            return panel;
        }
    }
}