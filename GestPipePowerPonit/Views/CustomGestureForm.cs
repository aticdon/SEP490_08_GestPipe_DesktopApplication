using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views; // ✅ Import CustomMessageBox
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class CustomGestureForm : Form
    {
        private TcpClient cameraClient;
        private NetworkStream cameraStream;
        private Thread cameraThread;

        private TcpClient statusClient;
        private NetworkStream statusStream;
        private Thread statusThread;
        private Process pythonProcess;
        private HomeUser _homeForm;

        private string userName;
        private string poseLabel;
        private string gestureName;
        private string gestureId;
        private string gestureTypeId;
        private string userId = Properties.Settings.Default.UserId;

        private int savedCount = 0;
        private const int CUSTOM_MAX = 5;
        private bool isRecording = false;

        // ✅ Loading components fields
        private int spinnerAngle = 0;
        private bool firstFrameReceived = false;

        public CustomGestureForm(HomeUser homeForm, string gestureId, string userName, string poseLabel, string gestureName)
        {
            InitializeComponent();
            _homeForm = homeForm;
            this.userName = userName;
            this.poseLabel = poseLabel;
            this.gestureName = gestureName;
            this.gestureId = gestureId;

            this.Load += CustomGestureForm_Load;
            lblName.Text = gestureName;
            UpdateInstructionTexts();
            GestPipePowerPonit.CultureManager.CultureChanged += (s, e) =>
            {
                RefreshUILanguage();
            };
        }
        // ✅ THÊM: Method để refresh tất cả UI elements
        private void RefreshUILanguage()
        {
            // Update instruction texts
            UpdateInstructionTexts();

            // Update user/pose info
            string userLabel = GetLocalizedText("user_label", "User:", "Người dùng:");
            string poseLabel = GetLocalizedText("pose_label", "Pose:", "Tư thế:");
            lblCustomInfo.Text = $"{userLabel} {userName}\r\n{poseLabel} {this.poseLabel}";

            // Update initial status
            UpdateInitialStatus();

            // Update count if already recording
            if (isRecording && savedCount > 0)
            {
                bool isVietnamese = IsVietnamese();
                lblCustomCount.Text = isVietnamese ?
                    $"Đã ghi: {savedCount}/5" :
                    $"Recorded: {savedCount}/5";
            }
        }

        private async void CustomGestureForm_Load(object sender, EventArgs e)
        {
            try
            {
                string userLabel = GetLocalizedText("user_label", "User:", "Người dùng:");
                string poseLabel = GetLocalizedText("pose_label", "Pose:", "Tư thế:");

                //lblCustomInfo.Text = $"User: {userName}\r\nPose: {poseLabel}";
                lblCustomInfo.Text = $"{userLabel} {userName}\r\n{poseLabel} {this.poseLabel}";

                // ✅ Apply language to initial status
                UpdateInitialStatus();
                CreateInstructionIcon();

                var defaultGestureService = new DefaultGestureService();
                //var gestureDetail = await defaultGestureService.GetGestureDetailAsync(gestureId);
                var gestureDetail = await defaultGestureService.GetDefaultGestureByid(gestureId);

                if (gestureDetail != null)
                {
                    gestureTypeId = gestureDetail.GestureTypeId;
                }
                else
                {
                    throw new Exception("Cannot load gesture details - gesture not found");
                }

                //var gestureDetail = await new UserGestureConfigService().GetUserGestureByid(gestureId);
                //gestureTypeId = gestureDetail.GestureTypeId;
            }
            catch (Exception ex)
            {
                string errorMsg = GetLocalizedText("error_init", "Error initializing CustomGesture: ", "Lỗi khi khởi tạo CustomGesture: ");
                CustomMessageBox.ShowError(errorMsg + ex.Message);
            }
        }

        // ✅ UPDATE: Localized initial status
        private void UpdateInitialStatus()
        {
            string statusMsg = GetLocalizedText("ready_to_start",
                "Press 'Start Recording' to activate camera",
                "Nhấn 'Bắt đầu ghi' để khởi động camera");
            lblCustomStatus.Text = statusMsg;
            lblCustomStatus.ForeColor = Color.Yellow;
        }

        // ✅ THÊM: Spinner animation method
        private void SpinnerTimer_Tick(object sender, EventArgs e)
        {
            spinnerAngle += 15; // Rotate 15 degrees each tick
            if (spinnerAngle >= 360) spinnerAngle = 0;
            DrawSpinner();
        }

        // ✅ THÊM: Draw spinner method
        private void DrawSpinner()
        {
            if (loadingSpinner.Image != null)
            {
                loadingSpinner.Image.Dispose();
            }

            Bitmap spinnerBitmap = new Bitmap(loadingSpinner.Width, loadingSpinner.Height);
            using (Graphics g = Graphics.FromImage(spinnerBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Draw spinning circle
                int centerX = loadingSpinner.Width / 2;
                int centerY = loadingSpinner.Height / 2;
                int radius = 20;

                for (int i = 0; i < 8; i++)
                {
                    double angle = (spinnerAngle + i * 45) * Math.PI / 180;
                    int x = centerX + (int)(Math.Cos(angle) * radius);
                    int y = centerY + (int)(Math.Sin(angle) * radius);

                    int alpha = 255 - (i * 30); // Fade effect
                    if (alpha < 0) alpha = 0;

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
                    {
                        g.FillEllipse(brush, x - 3, y - 3, 6, 6);
                    }
                }
            }

            loadingSpinner.Image = spinnerBitmap;
        }

        // ✅ THÊM: Update loading text
        private void UpdateLoadingText()
        {
            string loadingText = GetLocalizedText("loading_camera",
                "Starting camera and Python...\nPlease wait...",
                "Đang khởi động camera và Python...\nVui lòng đợi...");

            loadingLabel.Text = loadingText;

            // Center the label
            loadingLabel.Location = new Point(
                (loadingPanel.Width - loadingLabel.Width) / 2,
                loadingSpinner.Bottom + 20
            );
        }

        // ✅ THÊM: Show loading
        private void ShowLoading()
        {
            UpdateLoadingText();
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            spinnerTimer.Start();
            firstFrameReceived = false;
        }

        // ✅ THÊM: Hide loading
        private void HideLoading()
        {
            loadingPanel.Visible = false;
            spinnerTimer.Stop();
        }

        // ✅ SỬA: Button click to async
        private async void btnStartRecording_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                await StartRecordingProcessAsync();
                btnHome.Enabled = false;
            }
        }

        // ✅ SỬA: Đổi tên và chuyển thành async
        private async Task StartRecordingProcessAsync()
        {
            try
            {
                isRecording = true;
                btnStartRecording.Enabled = false;

                // ✅ HIỂN THỊ loading ngay lập tức
                ShowLoading();

                string startingText = GetLocalizedText("starting_btn", "⏳ Starting...", "⏳ Khởi động...");
                btnStartRecording.Text = startingText;

                EnsurePythonDirectoriesExist();
                // ✅ Chạy Python process trong background
                await Task.Run(() =>
                {
                    StartPythonProcess();
                    Thread.Sleep(2000); // Đợi Python khởi động
                    SendUserAndPoseToPython(userId, poseLabel);
                });

                StartReceivingCameraFrames(6001);
                StartReceivingCustomStatus(6002);

                // ✅ Đợi camera kết nối
                await WaitForCameraConnectionAsync();

                string recordingText = GetLocalizedText("recording_btn", "✅ Recording", "✅ Đang ghi");
                btnStartRecording.Text = recordingText;
                btnStartRecording.FillColor = Color.FromArgb(76, 175, 80);
            }
            catch (Exception ex)
            {
                HideLoading();

                string errorMsg = GetLocalizedText("error_start_recording",
                    "Error starting recording: ",
                    "Lỗi khi bắt đầu ghi: ");
                CustomMessageBox.ShowError(errorMsg + ex.Message);

                isRecording = false;
                btnStartRecording.Enabled = true;
                UpdateInstructionTexts();
            }
        }

        // ✅ THÊM: Đợi camera kết nối
        private async Task WaitForCameraConnectionAsync()
        {
            int timeout = 10000; // 10 seconds timeout
            int elapsed = 0;
            int checkInterval = 200;

            while (elapsed < timeout)
            {
                if (firstFrameReceived)
                {
                    return;
                }

                await Task.Delay(checkInterval);
                elapsed += checkInterval;
            }

            // Timeout - ẩn loading và hiện warning
            HideLoading();
            string timeoutMsg = GetLocalizedText("camera_timeout",
                "Camera connection timeout, but continuing...",
                "Kết nối camera timeout, nhưng vẫn tiếp tục...");

            lblCustomStatus.Text = timeoutMsg;
            lblCustomStatus.ForeColor = Color.Orange;
        }

        private void UpdateInstructionTexts()
        {
            try
            {
                ResourceHelper.SetCulture(CultureManager.CurrentCultureCode, this);

                lblInstructionTitle.Text = Properties.Resources.CustomGesture_Title;
                lblInstruction1.Text = Properties.Resources.CustomGesture_Instruction_01;
                lblInstruction2.Text = Properties.Resources.CustomGesture_Instruction_02;
                lblInstruction3.Text = Properties.Resources.CustomGesture_Instruction_03;
                lblInstruction4.Text = Properties.Resources.CustomGesture_Instruction_04;
                lblInstruction5.Text = Properties.Resources.CustomGesture_Instruction_05;
                lblInstruction6.Text = Properties.Resources.CustomGesture_Instruction_06;

                btnStartRecording.Text = Properties.Resources.BtnStartRecord;
                btnHome.Text = Properties.Resources.BtnBack;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ApplyResourceLanguage] Error: {ex.Message}");
                // Fallback to UpdateInstructionTexts
                UpdateInstructionTexts();
            }
        }

        private void CreateInstructionIcon()
        {
            try
            {
                var bmp = new Bitmap(35, 35);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);

                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Point(0, 0),
                        new Point(35, 35),
                        Color.FromArgb(0, 188, 212),
                        Color.FromArgb(0, 150, 170)))
                    {
                        g.FillEllipse(brush, 0, 0, 35, 35);
                    }

                    using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(Color.White))
                    {
                        var text = "✋";
                        var size = g.MeasureString(text, font);
                        var x = (35 - size.Width) / 2;
                        var y = (35 - size.Height) / 2;
                        g.DrawString(text, font, textBrush, x, y);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating instruction icon: {ex.Message}");
            }
        }

        // ✅ NEW: Helper method for localized text
        private string GetLocalizedText(string key, string englishText, string vietnameseText)
        {
            return IsVietnamese() ? vietnameseText : englishText;
        }

        private bool IsVietnamese()
        {
            try
            {
                return GestPipePowerPonit.CultureManager.CurrentCultureCode.Contains("vi") ||
                       AppSettings.CurrentLanguage == "VN";
            }
            catch
            {
                return true; // Default to Vietnamese
            }
        }

        private void SendUserAndPoseToPython(string user, string pose)
        {
            int retry = 0;
            while (retry < 10)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect("127.0.0.1", 7000);
                        using (var stream = client.GetStream())
                        {
                            string data = $"{user}|{pose}";
                            byte[] bytes = Encoding.UTF8.GetBytes(data);
                            stream.Write(bytes, 0, bytes.Length);
                            byte[] resp = new byte[8];
                            int len = stream.Read(resp, 0, resp.Length);
                            Debug.WriteLine($"[CustomGesture] Received socket response: {Encoding.UTF8.GetString(resp, 0, len)}");
                        }
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[CustomGesture] Error sending socket at retry {retry}: {e.Message}");
                    Thread.Sleep(500);
                    retry++;
                }
            }

            string errorMsg = GetLocalizedText("error_send_python",
                "Cannot send username/pose to Python server after 10 attempts.",
                "Không thể gửi username/pose sang Python server sau 10 lần thử.");
            CustomMessageBox.ShowError(errorMsg);
        }

        private void StartPythonProcess()
        {
            try
            {
                string pythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";
                string scriptFile = @"D:\Semester9\codepython\hybrid_realtime_pipeline\code\collect_data_update.py";

                if (!File.Exists(scriptFile))
                {
                    string errorMsg = GetLocalizedText("error_script_not_found",
                        "Python script file not found: ",
                        "Không tìm thấy file Python script: ");
                    CustomMessageBox.ShowError(errorMsg + scriptFile);
                    return;
                }

                if (pythonProcess != null && !pythonProcess.HasExited) return;

                pythonProcess = new Process();
                pythonProcess.StartInfo.FileName = pythonExePath;
                pythonProcess.StartInfo.Arguments = $"\"{scriptFile}\"";

                string scriptDirectory = Path.GetDirectoryName(scriptFile);
                pythonProcess.StartInfo.WorkingDirectory = scriptDirectory;

                pythonProcess.StartInfo.UseShellExecute = false;
                pythonProcess.StartInfo.RedirectStandardOutput = true;
                pythonProcess.StartInfo.RedirectStandardError = true;
                pythonProcess.StartInfo.CreateNoWindow = true;

                // ✅ SỬA: EnableRaisingEvents thuộc về Process, KHÔNG phải ProcessStartInfo
                // pythonProcess.StartInfo.EnableRaisingEvents = true;  ❌ SAI
                pythonProcess.EnableRaisingEvents = true;  // ✅ ĐÚNG

                // ✅ THÊM: Set UTF-8 encoding environment variables
                pythonProcess.StartInfo.EnvironmentVariables.Clear();
                foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
                {
                    pythonProcess.StartInfo.EnvironmentVariables.Add(env.Key.ToString(), env.Value.ToString());
                }

                // ✅ UTF-8 encoding settings
                pythonProcess.StartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
                pythonProcess.StartInfo.EnvironmentVariables["PYTHONUTF8"] = "1";

                // Existing environment variables
                pythonProcess.StartInfo.EnvironmentVariables["PYTHONPATH"] = scriptDirectory;
                pythonProcess.StartInfo.EnvironmentVariables["WINFORM_USER_ID"] = userId;
                pythonProcess.StartInfo.EnvironmentVariables["WINFORM_POSE_LABEL"] = poseLabel;
                pythonProcess.StartInfo.EnvironmentVariables["WINFORM_SCRIPT_DIR"] = scriptDirectory;

                Debug.WriteLine($"[CustomGesture] Python Working Directory: {scriptDirectory}");
                Debug.WriteLine($"[CustomGesture] Script Path: {scriptFile}");
                Debug.WriteLine($"[CustomGesture] Current C# Working Directory: {Directory.GetCurrentDirectory()}");

                pythonProcess.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        try
                        {
                            Debug.WriteLine("[PYTHON STDOUT] " + e.Data);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[PYTHON STDOUT] Error displaying output: {ex.Message}");
                        }
                    }
                };

                pythonProcess.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        try
                        {
                            Debug.WriteLine("[PYTHON STDERR] " + e.Data);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[PYTHON STDERR] Error displaying error: {ex.Message}");
                        }
                    }
                };

                pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();

                Debug.WriteLine("[CustomGesture] Started Python process: " + scriptFile);
            }
            catch (Exception ex)
            {
                string errorMsg = GetLocalizedText("error_python_start",
                    "Error starting Python script: ",
                    "Lỗi khi mở Python script: ");
                CustomMessageBox.ShowError(errorMsg + ex.Message);
            }
        }
        private void EnsurePythonDirectoriesExist()
        {
            try
            {
                string scriptDirectory = @"D:\Semester9\codepython\hybrid_realtime_pipeline\code";
                string userDirectory = Path.Combine(scriptDirectory, $"user_{userId}");
                string rawDataDirectory = Path.Combine(userDirectory, "raw_data");

                if (!Directory.Exists(userDirectory))
                {
                    Directory.CreateDirectory(userDirectory);
                    Debug.WriteLine($"Created user directory: {userDirectory}");
                }

                if (!Directory.Exists(rawDataDirectory))
                {
                    Directory.CreateDirectory(rawDataDirectory);
                    Debug.WriteLine($"Created raw data directory: {rawDataDirectory}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating directories: {ex.Message}");
            }
        }

        // ✅ SỬA: StartReceivingCameraFrames để ẩn loading khi nhận frame đầu tiên
        private void StartReceivingCameraFrames(int port)
        {
            if (cameraThread != null && cameraThread.IsAlive) return;
            cameraThread = new Thread(() =>
            {
                try
                {
                    Debug.WriteLine("[CustomGesture] Connecting camera socket...");
                    cameraClient = new TcpClient("127.0.0.1", port);
                    cameraStream = cameraClient.GetStream();
                    Debug.WriteLine("[CustomGesture] Camera socket connected.");

                    while (true)
                    {
                        byte[] lenBuf = new byte[4];
                        int read = 0;
                        while (read < 4)
                        {
                            int r = cameraStream.Read(lenBuf, read, 4 - read);
                            if (r <= 0)
                            {
                                Debug.WriteLine("[CustomGesture] Socket closed while reading length.");
                                return;
                            }
                            read += r;
                        }
                        int length = BitConverter.ToInt32(lenBuf.Reverse().ToArray(), 0);
                        if (length < 1000 || length > 1000000)
                        {
                            Debug.WriteLine("[CustomGesture] Skip frame (invalid length " + length + ").");
                            continue;
                        }
                        byte[] imgBuf = new byte[length];
                        read = 0;
                        while (read < length)
                        {
                            int r = cameraStream.Read(imgBuf, read, length - read);
                            if (r <= 0)
                            {
                                Debug.WriteLine("[CustomGesture] Socket closed while reading image.");
                                return;
                            }
                            read += r;
                        }
                        try
                        {
                            using (var ms = new MemoryStream(imgBuf))
                            {
                                var img = Image.FromStream(ms);
                                this.Invoke(new Action(() =>
                                {
                                    pictureBoxCustomCamera.Image = img;

                                    // ✅ Ẩn loading khi nhận frame đầu tiên
                                    if (!firstFrameReceived)
                                    {
                                        firstFrameReceived = true;
                                        HideLoading();

                                        // Set ready status
                                        string readyMsg = GetLocalizedText("ready_gesture",
                                            "Ready! Perform gesture according to instructions",
                                            "Sẵn sàng! Thực hiện gesture theo hướng dẫn");
                                        lblCustomStatus.Text = readyMsg;
                                        lblCustomStatus.ForeColor = Color.Lime;
                                    }
                                }));
                                Debug.WriteLine("[CustomGesture] Frame displayed OK.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("[CustomGesture] Error decode/display image: " + ex.Message);
                            this.Invoke(new Action(() => { pictureBoxCustomCamera.BackColor = Color.Red; }));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[CustomGesture] Error in camera thread: " + e.Message);
                    // ✅ Ẩn loading khi có lỗi
                    this.Invoke(() => HideLoading());
                }
            });
            cameraThread.IsBackground = true;
            cameraThread.Start();
        }

        private async void SaveCustomGestureRequest()
        {
            try
            {
                var dto = new UserGestureRequestDto
                {
                    UserId = userId,
                    UserGestureConfigId = gestureId,
                    GestureTypeId = gestureTypeId,
                    PoseLabel = poseLabel,
                    Status = new Dictionary<string, string> { { "en", "Pending" },
                    { "vi", "Đang xử lý" }}
                };

                var service = new UserGestureRequestService();
                bool success = await service.CreateRequestAsync(dto);

                if (success)
                {
                    string successMsg = GetLocalizedText("save_success",
                        "Custom gesture request saved to database (pending status).",
                        "Yêu cầu cử chỉ tùy chỉnh đã được lưu vào cơ sở dữ liệu (trạng thái đang chờ).");
                    MessageBox.Show(successMsg);
                }
                else
                {
                    string failMsg = GetLocalizedText("save_fail",
                        "Failed to save custom gesture request!",
                        "Không thể lưu yêu cầu cử chỉ tùy chỉnh!");
                    MessageBox.Show(failMsg);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = GetLocalizedText("save_error",
                    "Error saving custom gesture to DB: ",
                    "Lỗi khi lưu cử chỉ tùy chỉnh vào cơ sở dữ liệu: ");
                MessageBox.Show(errorMsg + ex.Message);
            }
        }

        // ✅ UPDATED: Enhanced to handle bilingual status messages
        private void StartReceivingCustomStatus(int port)
        {
            if (statusThread != null && statusThread.IsAlive) return;
            statusThread = new Thread(() =>
            {
                try
                {
                    Debug.WriteLine("[CustomGesture] Connecting status socket...");
                    statusClient = new TcpClient("127.0.0.1", port);
                    statusStream = statusClient.GetStream();
                    Debug.WriteLine("[CustomGesture] Status socket connected.");
                    byte[] buffer = new byte[1024]; // ✅ Increased buffer size for bilingual messages
                    while (true)
                    {
                        int received = statusStream.Read(buffer, 0, buffer.Length);
                        if (received > 0)
                        {
                            string text = Encoding.UTF8.GetString(buffer, 0, received);
                            Debug.WriteLine("[CustomGesture] Status received: " + text);
                            var parts = text.Split('|');

                            // ✅ NEW: Handle both old format (5 parts) and new bilingual format (6 parts)
                            if (parts.Length >= 5)
                            {
                                string eventName = parts[0];
                                string poseName = parts[1];
                                int parsedSavedCount;
                                bool okCount = int.TryParse(parts[2], out parsedSavedCount);
                                bool isConflict = parts[3] == "True";

                                string statusMessage;

                                if (parts.Length >= 6)
                                {
                                    // ✅ NEW FORMAT: Use appropriate language message
                                    string englishMessage = parts[4];
                                    string vietnameseMessage = parts[5];
                                    statusMessage = IsVietnamese() ? vietnameseMessage : englishMessage;
                                }
                                else
                                {
                                    // ✅ OLD FORMAT: Use single message (backward compatibility)
                                    statusMessage = parts[4];
                                }

                                this.Invoke(new Action(() =>
                                {
                                    lblCustomStatus.Text = statusMessage;
                                    savedCount = okCount ? parsedSavedCount : savedCount;

                                    bool isVietnamese = IsVietnamese();
                                    lblCustomCount.Text = isVietnamese ?
                                        $"Đã ghi: {savedCount}/5" :
                                        $"Recorded: {savedCount}/5";

                                    // ✅ Set appropriate colors based on event type
                                    if (eventName == "CONFLICT" || isConflict)
                                        lblCustomStatus.ForeColor = Color.Red;
                                    else if (eventName == "ERROR")
                                        lblCustomStatus.ForeColor = Color.OrangeRed;
                                    else if (eventName == "START")
                                        lblCustomStatus.ForeColor = Color.Blue;
                                    else if (eventName == "COLLECTED")
                                        lblCustomStatus.ForeColor = Color.Cyan;
                                    else
                                        lblCustomStatus.ForeColor = Color.LightCyan;

                                    // ✅ Handle completion
                                    if (eventName == "FINISH")
                                    {
                                        // Prepare success message
                                        string successMessage = GetLocalizedText("training_success",
                                            $"Training completed successfully!",
                                            $"Huấn luyện hoàn thành thành công!");

                                        string successTitle = GetLocalizedText("success_title", "Training Completed", "Hoàn thành huấn luyện");

                                        // Show custom success message
                                        var result = CustomMessageBox.ShowSuccess(successMessage, successTitle);
                                        SaveCustomGestureRequest();

                                        if (result == DialogResult.OK)
                                        {
                                            this.Hide();

                                            try
                                            {
                                                var customGestureForm = new ListRequestGestureForm(_homeForm);
                                                customGestureForm.Show();
                                                this.Close();
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine($"Error opening FormUserGestureCustom: {ex.Message}");
                                                // Fallback to home form
                                                _homeForm.Show();
                                                this.Close();
                                            }
                                        }
                                    }
                                }));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("[CustomGesture] Error in status thread: " + e.Message);
                }
            });
            statusThread.IsBackground = true;
            statusThread.Start();
        }

        // ✅ SỬA: OnFormClosing để cleanup timer
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // ✅ Stop loading animation
                spinnerTimer?.Stop();

                Debug.WriteLine("[CustomGesture] Closing connections to signal Python to exit...");

                cameraThread?.Abort();
                cameraStream?.Close();
                cameraClient?.Close();

                statusThread?.Abort();
                statusStream?.Close();
                statusClient?.Close();

                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    Debug.WriteLine("[CustomGesture] Waiting for Python to save files and exit gracefully...");
                    bool exitedGracefully = pythonProcess.WaitForExit(10000);

                    if (exitedGracefully)
                    {
                        Debug.WriteLine("[CustomGesture] Python exited gracefully - files should be saved!");
                    }
                    else
                    {
                        Debug.WriteLine("[CustomGesture] Python didn't exit in time, forcing kill...");
                        pythonProcess.Kill();
                        pythonProcess.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[CustomGesture] OnClose error: " + ex.Message);
            }
            base.OnFormClosing(e);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ListRequestGestureForm customGestureForm = new ListRequestGestureForm(_homeForm);
            customGestureForm.Show();
            this.Close();
        }
    }
}