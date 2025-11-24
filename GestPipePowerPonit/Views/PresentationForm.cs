using DnsClient.Protocol;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views.Auth;
using GestPipePowerPonit.Views.Profile;
using GestPipePowerPonit.Views;
using Microsoft.Office.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using A = DocumentFormat.OpenXml.Drawing;
using OpenXmlShape = DocumentFormat.OpenXml.Presentation.Shape;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using WinFormControl = System.Windows.Forms.Control;
using System.Security.AccessControl;

namespace GestPipePowerPonit
{
    public partial class PresentationForm : Form
    {
        private System.Windows.Forms.Timer autoSlideTimer;
        private SocketServer server;
        private Process pythonProcess = null;
        private HomeUser _homeForm;
        private SessionService sessionService = new SessionService();
        private CategoryService categoryService = new CategoryService();
        private TopicService topicService = new TopicService();
        private DateTime? _startTime = null;
        private string userId = Properties.Settings.Default.UserId;
        private readonly ApiClient _apiClient;
        private bool _isInitializing = false;


        // ✅ THÊM AuthService
        private readonly AuthService _authService;

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public PresentationForm(HomeUser homeForm)
        {
            InitializeComponent();
            btnViewLeft.Visible = false;
            btnViewRight.Visible = false;
            btnViewTop.Visible = false;
            btnViewBottom.Visible = false;
            btnZoomInTop.Visible = false;
            btnZoomOutTop.Visible = false;
            btnZoomInSlide.Visible = false;
            btnZoomOutSlide.Visible = false;

            this.AutoScaleMode = AutoScaleMode.Dpi;

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            autoSlideTimer = new System.Windows.Forms.Timer();

            webView2_3D = new WebView2();
            webView2_3D.Location = new Point(0, 0);
            webView2_3D.Size = panelSlide.Size;
            webView2_3D.Visible = false;
            webView2_3D.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelSlide.Controls.Add(webView2_3D);

            _homeForm = homeForm;

            if (btnLanguageEN != null)
                btnLanguageEN.Click += (s, e) => UpdateCultureAndApply("en-US");
            if (btnLanguageVN != null)
                btnLanguageVN.Click += (s, e) => UpdateCultureAndApply("vi-VN");

            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;
            cmbTopic.SelectedIndexChanged += cmbTopic_SelectedIndexChanged;
            // ✅ GẮN SỰ KIỆN LOGOUT VÀ PROFILE
            btnLogout.Click += btnLogout_Click;
            btnProfile.Click += btnProfile_Click;

            _apiClient = new ApiClient("https://localhost:7219");

            // ✅ KHỞI TẠO AuthService
            _authService = new AuthService();

            CultureManager.CultureChanged += async (s, e) =>
            {
                ApplyLanguage(CultureManager.CurrentCultureCode);
            };
        }
        private async void btnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PowerPoint Files|*.pptx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 🧹 1. DỌN TOÀN BỘ SESSION CŨ
                    CleanupResources(keepPython: true);

                    // 🧠 2. Reset state logic
                    zoomSlideCount = 0;
                    gestureCounts.Clear();
                    firstCameraFrameReceived = false;

                    txtFile.Text = openFileDialog1.FileName;
                    string ext = Path.GetExtension(txtFile.Text).ToLower();

                    // 🧷 3. Đăng ký hotkey lại (nếu cần)
                    RegisterKeys();

                    // 🛰 4. Tạo SocketServer mới
                    server = new SocketServer(5006, this, HandleGestureCommand);
                    server.Start();
                    Debug.WriteLine("[SocketServer] Started on port 5006");

                    // 🎥 5. Start camera receiver
                    ShowLoading(
                        CultureManager.CurrentCultureCode.Contains("vi")
                            ? "🎥 Đang kết nối camera..." : "🎥 Connecting camera..."
                    );
                    StartCameraReceiver(6000);
                    pictureBoxCamera.Visible = true;

                    // 🐍 6. Start Python (sau khi server & camera đã sẵn sàng)
                    //await Task.Run(() => StartPythonProcess());
                    Task.Run(() => StartPythonProcess());

                    if (ext == ".pptx")
                    {
                        webView2_3D.Visible = false;
                        SetButtonsEnabled(true, false);

                        await Task.Run(() =>
                        {
                            Extract3DModels(txtFile.Text);
                            slidesWith3D = slide3DModelFiles.Keys.ToList();
                        });

                        try
                        {
                            oPPT = new PowerPoint.Application();
                            oPres = oPPT.Presentations.Open(
                                txtFile.Text,
                                MsoTriState.msoFalse,
                                MsoTriState.msoFalse,
                                MsoTriState.msoFalse
                            );

                            oPPT.SlideShowNextSlide += O_PPT_SlideShowNextSlide;
                            btnSlideShow.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                                ? $"Không thể mở PowerPoint: {ex.Message}"
                                : $"Cannot open PowerPoint: {ex.Message}";
                            Console.WriteLine(errorMsg);
                            oPPT = null;
                            oPres = null;
                            btnSlideShow.Enabled = false;
                        }
                    }

                    await WaitForCameraConnectionAsync();
                    HideLoading();

                    btnHome.Enabled = false;
                    btnGestureControl.Enabled = false;
                    btnInstruction.Enabled = false;
                    btnPresentation.Enabled = false;
                    btnCustomGesture.Enabled = false;
                    btnProfile.Enabled = false;
                }
                catch (Exception ex)
                {
                    string errorMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? $"Lỗi: {ex.Message}"
                        : $"Error: {ex.Message}";
                    Console.WriteLine(errorMsg);
                    HideLoading();
                }
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case 1: btnFirst_Click(null, null); break;
                    case 2: btnPrev_Click(null, null); break;
                    case 3: btnNext_Click(null, null); break;
                    case 4: btnLast_Click(null, null); break;
                    case 5: btnPause_Click(null, null); break;
                    case 6: btnClose_Click(null, null); break;
                    case 7: btnViewLeft_Click(null, null); break;
                    case 8: btnViewRight_Click(null, null); break;
                    case 9: btnViewTop_Click(null, null); break;
                    case 10: btnViewBottom_Click(null, null); break;
                    case 11: btnZoomInTop_Click(null, null); break;
                    case 12: btnZoomOutTop_Click(null, null); break;
                    case 13: btnZoomInSlide_Click(null, null); break;
                    case 14: btnZoomOutSlide_Click(null, null); break;
                }
            }
            base.WndProc(ref m);
        }

        private void RegisterKeys()
        {
            RegisterHotKey(this.Handle, 1, 0, (int)Keys.D1);
            RegisterHotKey(this.Handle, 2, 0, (int)Keys.D2);
            RegisterHotKey(this.Handle, 3, 0, (int)Keys.D3);
            RegisterHotKey(this.Handle, 4, 0, (int)Keys.D4);
            RegisterHotKey(this.Handle, 5, 0, (int)Keys.D5);
            RegisterHotKey(this.Handle, 6, 0, (int)Keys.Escape);
            RegisterHotKey(this.Handle, 7, 0, (int)Keys.Q);
            RegisterHotKey(this.Handle, 8, 0, (int)Keys.W);
            RegisterHotKey(this.Handle, 9, 0, (int)Keys.E);
            RegisterHotKey(this.Handle, 10, 0, (int)Keys.R);
            RegisterHotKey(this.Handle, 11, 0, (int)Keys.A);
            RegisterHotKey(this.Handle, 12, 0, (int)Keys.S);
            RegisterHotKey(this.Handle, 13, 0, (int)Keys.Z);
            RegisterHotKey(this.Handle, 14, 0, (int)Keys.X);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            UnregisterKeys();
            base.OnHandleDestroyed(e);
        }

        private void UnregisterKeys()
        {
            for (int i = 1; i <= 14; i++)
                UnregisterHotKey(this.Handle, i);
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (panelSlide.Parent == this && !isFullScreen)
            {
                panelSlide.Size = new Size((int)(ClientSize.Width * 0.98f), (int)(ClientSize.Height * 0.74f));
                panelSlide.Location = new Point((ClientSize.Width - panelSlide.Width) / 2,
                                                (int)(ClientSize.Height * 0.24f));
                webView2_3D.Size = panelSlide.Size;
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1: case Keys.NumPad1: btnFirst.PerformClick(); break;
                case Keys.D2: case Keys.NumPad2: btnPrev.PerformClick(); break;
                case Keys.D3: case Keys.NumPad3: btnNext.PerformClick(); break;
                case Keys.D4: case Keys.NumPad4: btnLast.PerformClick(); break;
                case Keys.D5: case Keys.NumPad5: btnPause.PerformClick(); break;
                case Keys.D6: case Keys.NumPad6: btnClose.PerformClick(); break;
                case Keys.Q: btnViewLeft_Click(null, null); break;
                case Keys.W: btnViewRight_Click(null, null); break;
                case Keys.E: btnViewTop_Click(null, null); break;
                case Keys.R: btnViewBottom_Click(null, null); break;
                case Keys.A: btnZoomInTop_Click(null, null); break;
                case Keys.S: btnZoomOutTop_Click(null, null); break;
                case Keys.Z: btnZoomInSlide_Click(null, null); break;
                case Keys.X: btnZoomOutSlide_Click(null, null); break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CleanupResources(keepPython: false);
            base.OnFormClosing(e);
        }

        private void panelSlide_Paint(object sender, PaintEventArgs e) { }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _isInitializing = true;
            ApplyLanguage(GestPipePowerPonit.CultureManager.CurrentCultureCode);
            pictureBoxCamera.Visible = false;
            SetButtonsEnabled(false, false);

            var categories = await categoryService.GetCategoriesAsync();

            cmbCategory.DisplayMember = "DisplayName";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DataSource = categories;

            _ = Task.Run(() => StartPythonProcess());
            // 🔁 Restore LastSelectedCategoryId
            var lastCategoryId = Properties.Settings.Default.LastSelectedCategoryId;
            if (!string.IsNullOrEmpty(lastCategoryId) &&
                categories.Any(c => c.Id == lastCategoryId))
            {
                cmbCategory.SelectedValue = lastCategoryId;
            }
            else if (categories.Count > 0)
            {
                cmbCategory.SelectedIndex = 0;
            }

            // Sau khi chọn được category -> load topic
            await LoadTopicsForSelectedCategoryAsync();
            _isInitializing = false;
        }

        private void HandleGestureCommand(string command)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleGestureCommand(command)));
                return;
            }

            Debug.WriteLine($"[HandleGestureCommand] Received: {command}");
            if (gestureCounts.ContainsKey(command))
                gestureCounts[command]++;
            else
                gestureCounts[command] = 1;
            switch (command)
            {
                case "zoom_in": btnZoomInTop_Click(null, null); Console.WriteLine("Zoom in"); break;
                case "zoom_out": btnZoomOutTop_Click(null, null); Console.WriteLine("Zoom out"); break;
                case "next_slide": btnNext_Click(null, null); Console.WriteLine("Next Slide"); break;
                case "previous_slide": btnPrev_Click(null, null); Console.WriteLine("Previous Slide"); break;
                case "home": btnFirst_Click(null, null); Console.WriteLine("Home"); break;
                case "end": btnLast_Click(null, null); Console.WriteLine("End"); break;
                case "rotate_left": btnViewRight_Click(null, null); Console.WriteLine("Rotate Left"); break;
                case "rotate_right": btnViewLeft_Click(null, null); Console.WriteLine("Rotate Right"); break;
                case "rotate_up": btnViewBottom_Click(null, null); Console.WriteLine("Rotate Up"); break;
                case "start_present": btnSlideShow_Click(null, null); Console.WriteLine("Slide Show"); break;
                case "end_present": btnClose_Click(null, null); Console.WriteLine("Close Slide"); break;
                case "rotate_down": btnViewTop_Click(null, null); Console.WriteLine("Rotate Down"); break;
                case "zoom_in_slide": btnZoomInSlide_Click(null, null); Console.WriteLine("Zoom In Slide"); break;
                case "zoom_out_slide": btnZoomOutSlide_Click(null, null); Console.WriteLine("Zoom Out Slide"); break;
                default: Console.WriteLine($"Nhận lệnh không xác định: {command}"); break;
            }
        }
        private void StartPythonProcess()
        {
            try
            {
                if (pythonProcess != null)
                {
                    try
                    {
                        if (!pythonProcess.HasExited)
                        {
                            Debug.WriteLine("Python process đã chạy rồi.");
                            return;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    pythonProcess.Dispose();
                    pythonProcess = null;
                }

                string pythonExePath = "python";
                string userFolder = $"user_{userId}";
                //string userFolder = "user_691d8197db57fd91994a04f3";
                string scriptFile = $@"D:\Semester9\codepython\hybrid_realtime_pipeline\code\{userFolder}\test_gesture_recognition.py";

                Debug.WriteLine("Python exe path: " + pythonExePath);
                Debug.WriteLine("Python script path: " + scriptFile);

                if (!File.Exists(scriptFile))
                {
                    Debug.WriteLine("Không tìm thấy script python: " + scriptFile);
                    return;
                }

                var proc = new Process();
                proc.StartInfo.FileName = pythonExePath;
                proc.StartInfo.Arguments = $"\"{scriptFile}\"";
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptFile);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;

                proc.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON OUT] " + e.Data);
                };
                proc.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON ERR] " + e.Data);
                };

                bool started = proc.Start();
                if (!started)
                {
                    Debug.WriteLine("Failed to start Python process.");
                    proc.Dispose();
                    return;
                }

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                pythonProcess = proc;  // 🔑 chỉ gán sau khi Start OK

                Debug.WriteLine($"Started Python process: {pythonExePath} {proc.StartInfo.Arguments}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi chạy Python: " + ex.ToString());
            }
        }


        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }
        private async void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializing) return;               
            if (cmbCategory.SelectedValue == null) return;

            // 💾 Save Category vào Settings
            Properties.Settings.Default.LastSelectedCategoryId = cmbCategory.SelectedValue.ToString();
            Properties.Settings.Default.Save();

            await LoadTopicsForSelectedCategoryAsync();
        }
        private void cmbTopic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializing) return;               
            if (cmbTopic.SelectedValue == null) return;

            Properties.Settings.Default.LastSelectedTopicId = cmbTopic.SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }
        private async void UpdateCultureAndApply(string cultureCode)
        {
            try
            {
                CultureManager.CurrentCultureCode = cultureCode;
                ResourceHelper.SetCulture(cultureCode, this);

                // 🔁 Lấy lại danh sách category theo ngôn ngữ mới
                var categories = await categoryService.GetCategoriesAsync();

                // Lưu lại category hiện tại (nếu có) để giữ nguyên lựa chọn
                var lastCategoryId = Properties.Settings.Default.LastSelectedCategoryId;

                cmbCategory.DataSource = null;
                cmbCategory.DisplayMember = "DisplayName";
                cmbCategory.ValueMember = "Id";
                cmbCategory.DataSource = categories;

                // Restore category đã chọn trước đó (nếu vẫn tồn tại)
                if (!string.IsNullOrEmpty(lastCategoryId) &&
                    categories.Any(c => c.Id == lastCategoryId))
                {
                    cmbCategory.SelectedValue = lastCategoryId;
                }
                else if (categories.Count > 0)
                {
                    cmbCategory.SelectedIndex = 0;
                }
                await LoadTopicsForSelectedCategoryAsync();

                await _apiClient.SetUserLanguageAsync(userId, cultureCode);
            }
            catch (Exception ex)
            {
            }
        }

        public void ApplyLanguage(string cultureCode)
        {
            ResourceHelper.SetCulture(cultureCode, this);
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            lblPresentationFile.Text = Properties.Resources.Lbl_PresentiontationFile;
            lblOpenFile.Text = Properties.Resources.LblOpenFile;
            lblCategory.Text = Properties.Resources.Lbl_Category;
            lblTopic.Text = Properties.Resources.Lbl_Topic;
            btnSlideShow.Text = Properties.Resources.Btn_SlideShow;
            //btnExit.Text = Properties.Resources.Btn_Exit;
            lblPresentationPreview.Text = Properties.Resources.Lbl_PresentationPreview;
            //btnClose.Text = Properties.Resources.LblClose;
            btnOpen.Text = Properties.Resources.Btn_Browse;
            btnExit.Text = Properties.Resources.LblClose;
        }

        private void btnGestureControl_Click(object sender, EventArgs e)
        {
            CleanupResources(keepPython: false);
            ListDefaultGestureForm dGestureForm = new ListDefaultGestureForm(_homeForm);
            dGestureForm.Show();
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
                CleanupResources(keepPython: false);
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

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("[PresentationForm] LOGOUT PROCESS STARTED");
                Console.WriteLine(new string('=', 60));

                // Cleanup resources
                CleanupResources(keepPython: false);

                var response = await _authService.LogoutAsync();

                if (response?.Success == true)
                {
                    Console.WriteLine("[PresentationForm] ✅ Logout successful");

                    // ✅ Update loading message
                    string successMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? "✅ Đăng xuất thành công!"
                        : "✅ Logout successful!";
                    UpdateLoadingText(successMsg);

                    await Task.Delay(1000);

                    HideLoading();

                    CustomMessageBox.ShowSuccess(
                        Properties.Resources.Message_LogoutSuccess,
                        Properties.Resources.Title_Success
                    );

                    var loginForm = new LoginForm();
                    _homeForm?.Close();
                    this.Hide();
                    loginForm.Show();
                    this.Dispose();
                }
                else
                {
                    HideLoading();

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
                HideLoading();

                CustomMessageBox.ShowError(
                    $"{Properties.Resources.Message_LogoutError}: {ex.Message}",
                    Properties.Resources.Title_ConnectionError
                );

                btnLogout.Enabled = true;
                btnProfile.Enabled = true;
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                CleanupResources(keepPython: false);
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
        private async Task LoadTopicsForSelectedCategoryAsync()
        {
            var categoryId = cmbCategory.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(categoryId))
            {
                cmbTopic.DataSource = null;
                return;
            }

            var allTopics = await topicService.GetTopicsAsync();
            var filteredTopics = allTopics
                .Where(t => t.CategoryId == categoryId)
                .ToList();

            cmbTopic.DisplayMember = "DisplayTitle";
            cmbTopic.ValueMember = "Id";
            cmbTopic.DataSource = filteredTopics;

            // 🔁 Restore LastSelectedTopicId nếu còn tồn tại
            var lastTopicId = Properties.Settings.Default.LastSelectedTopicId;
            if (!string.IsNullOrEmpty(lastTopicId) &&
                filteredTopics.Any(t => t.Id == lastTopicId))
            {
                cmbTopic.SelectedValue = lastTopicId;
            }
        }
        private void CleanupResources(bool keepPython)
        {
            try
            {
                try { spinnerTimer?.Stop(); } catch { }

                // Camera
                try { StopCameraReceiver(); }
                catch (Exception ex)
                {
                    Debug.WriteLine("[Cleanup] StopCameraReceiver error: " + ex.Message);
                }

                // ✅ Python – chỉ kill nếu không keepPython
                if (!keepPython && pythonProcess != null)
                {
                    try
                    {
                        try
                        {
                            if (!pythonProcess.HasExited)
                                pythonProcess.Kill();
                        }
                        catch (InvalidOperationException)
                        {
                        }

                        pythonProcess.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[Cleanup] pythonProcess error: " + ex.Message);
                    }
                    finally
                    {
                        pythonProcess = null;
                    }
                }

                // Socket server
                if (server != null)
                {
                    try { server.Stop(); }
                    catch (Exception ex) { Debug.WriteLine("[Cleanup] SocketServer stop error: " + ex.Message); }
                    finally { server = null; }
                }

                // PowerPoint ...
                // (giữ nguyên phần oPPT, oPres như bạn đã làm)

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cleaning up resources: {ex.Message}");
            }
        }
        private void btnCustomGesture_Click(object sender, EventArgs e)
        {
            CleanupResources(keepPython: false);
            ListRequestGestureForm uGestureForm = new ListRequestGestureForm(_homeForm);
            uGestureForm.Show();
            this.Hide();

        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            // 1. Dọn tài nguyên giống đóng form
            CleanupResources(keepPython: true);

            // 2. Reset trạng thái logic
            _startTime = null;
            firstCameraFrameReceived = false;
            gestureCounts.Clear();

            // 3. Reset UI
            txtFile.Text = string.Empty;
            lblSlide.Text = "Slide - / -";

            // Ẩn camera + 3D
            if (pictureBoxCamera.Image != null)
            {
                pictureBoxCamera.Image.Dispose();
                pictureBoxCamera.Image = null;
            }
            pictureBoxCamera.Visible = false;

            webView2_3D.Visible = false;

            // Không có file, không slideshow
            SetButtonsEnabled(false, false);

            // Bật lại các nút menu bên trái để user chọn lại
            btnHome.Enabled = true;
            btnGestureControl.Enabled = true;
            btnInstruction.Enabled = true;
            btnPresentation.Enabled = true;
            btnCustomGesture.Enabled = true;
            btnProfile.Enabled = true;
            btnOpen.Enabled = true;

            // Nếu đang hiện loading panel thì tắt luôn cho chắc
            HideLoading();

            Debug.WriteLine("[PresentationForm] Clear clicked – state reset without closing form");
        }
        private void btnInstruction_Click(object sender, EventArgs e)
        {
            CleanupResources(keepPython: false);
            InstructionForm instructionForm = new InstructionForm(_homeForm);
            instructionForm.Show();
            this.Hide();
        }
    }
}