using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;
using GestPipePowerPonit.Views; // ✅ Import CustomMessageBox
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class FormCustomGesture : Form
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

        private int savedCount = 0;
        private const int CUSTOM_MAX = 5;
        private bool isRecording = false;

        public FormCustomGesture(HomeUser homeForm, string gestureId,string userName, string poseLabel, string gestureName)
        {
            InitializeComponent();
            _homeForm = homeForm;
            this.userName = userName;
            this.poseLabel = poseLabel;
            this.gestureName = gestureName;
            this.gestureId = gestureId;

            this.Load += FormCustomGesture_Load;
            lblName.Text = gestureName;
            UpdateInstructionTexts();
        }

        private async void FormCustomGesture_Load(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine($"[CustomGesture] Form loaded. User: {userName}, Pose: {poseLabel}");

                lblCustomInfo.Text = $"User: {userName}\r\n\r\nPose: {poseLabel}";

                // ✅ Apply language to initial status
                UpdateInitialStatus();
                CreateInstructionIcon();

                var gestureDetail = await new UserGestureConfigService().GetUserGestureByid(gestureId);
                gestureTypeId = gestureDetail.GestureTypeId;
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

        private void btnStartRecording_Click(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                StartRecordingProcess();
            }
        }

        private void StartRecordingProcess()
        {
            try
            {
                isRecording = true;
                btnStartRecording.Enabled = false;

                // ✅ UPDATE: Localized button text
                string startingText = GetLocalizedText("starting_btn", "⏳ Starting...", "⏳ Khởi động...");
                btnStartRecording.Text = startingText;

                // ✅ UPDATE: Localized status message
                string statusMsg = GetLocalizedText("starting_camera",
                    "Starting camera and Python...",
                    "Đang khởi động camera và Python...");
                lblCustomStatus.Text = statusMsg;
                lblCustomStatus.ForeColor = Color.Yellow;

                StartPythonProcess();
                SendUserAndPoseToPython(userName, poseLabel);
                StartReceivingCameraFrames(6001);
                StartReceivingCustomStatus(6002);

                // ✅ UPDATE: Localized ready status
                string readyMsg = GetLocalizedText("ready_gesture",
                    "Ready! Perform gesture according to instructions",
                    "Sẵn sàng! Thực hiện gesture theo hướng dẫn");
                lblCustomStatus.Text = readyMsg;
                lblCustomStatus.ForeColor = Color.Lime;

                string recordingText = GetLocalizedText("recording_btn", "✅ Recording", "✅ Đang ghi");
                btnStartRecording.Text = recordingText;
                btnStartRecording.FillColor = Color.FromArgb(76, 175, 80);
            }
            catch (Exception ex)
            {
                string errorMsg = GetLocalizedText("error_start_recording",
                    "Error starting recording: ",
                    "Lỗi khi bắt đầu ghi: ");
                CustomMessageBox.ShowError(errorMsg + ex.Message);

                isRecording = false;
                btnStartRecording.Enabled = true;
                UpdateInstructionTexts(); // Reset button text
            }
        }

        private void UpdateInstructionTexts()
        {
            bool isVietnamese = IsVietnamese();

            if (isVietnamese)
            {
                lblInstructionTitle.Text = "📋 Hướng dẫn thực hiện";
                lblInstruction1.Text = "🎯 Giữ tay trong khung hình, cách camera 40–60 cm";
                lblInstruction2.Text = "✋ Không để tay ra ngoài mép khung";
                lblInstruction3.Text = "💡 Đảm bảo đủ ánh sáng";
                lblInstruction4.Text = "📵 Không để vật thể khác che tay";
                lblInstruction5.Text = "⏱ Giữ tư thế trong 0.8–1.0 giây để mẫu được ghi";
                lblInstruction6.Text = "🔄 Lặp lại 5 lần để đảm bảo chất lượng mẫu";
                btnStartRecording.Text = "🚀 Bắt đầu ghi";
                btnHome.Text = "Về Trang Chủ";
            }
            else
            {
                lblInstructionTitle.Text = "📋 Instructions";
                lblInstruction1.Text = "🎯 Keep hands within frame, 40–60 cm from camera";
                lblInstruction2.Text = "✋ Don't let hands go outside frame edges";
                lblInstruction3.Text = "💡 Ensure sufficient lighting";
                lblInstruction4.Text = "📵 Don't let other objects cover hands";
                lblInstruction5.Text = "⏱ Hold pose for 0.8–1.0 seconds for recording";
                lblInstruction6.Text = "🔄 Repeat 5 times to ensure sample quality";
                btnStartRecording.Text = "🚀 Start Recording";
                btnHome.Text = "Home";
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
                pythonProcess.EnableRaisingEvents = true;

                Debug.WriteLine($"[CustomGesture] Python Working Directory: {scriptDirectory}");
                Debug.WriteLine($"[CustomGesture] Script Path: {scriptFile}");

                pythonProcess.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON STDOUT] " + e.Data);
                };
                pythonProcess.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Debug.WriteLine("[PYTHON STDERR] " + e.Data);
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
                                this.Invoke(new Action(() => { pictureBoxCustomCamera.Image = img; }));
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
                    UserId = userName,
                    UserGestureConfigId = gestureId,
                    GestureTypeId = gestureTypeId,
                    PoseLabel = poseLabel,
                    Status = new Dictionary<string, string> { { "en", "Pending" },
                    { "vi", "Đang xử lý" }}
                };

                var service = new UserGestureRequestService();
                bool success = await service.CreateRequestAsync(dto);

                if (success)
                    MessageBox.Show("Custom gesture request saved to database (pending status).");
                else
                    MessageBox.Show("Failed to save custom gesture request!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving custom gesture to DB: " + ex.Message);
            }
        }

        // ✅ UPDATED: Use CustomMessageBox and navigate to FormUserGestureCustom
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
                    byte[] buffer = new byte[256];
                    while (true)
                    {
                        int received = statusStream.Read(buffer, 0, buffer.Length);
                        if (received > 0)
                        {
                            string text = Encoding.UTF8.GetString(buffer, 0, received);
                            Debug.WriteLine("[CustomGesture] Status received: " + text);
                            var parts = text.Split('|');
                            if (parts.Length >= 5)
                            {
                                string eventName = parts[0];
                                string poseName = parts[1];
                                int parsedSavedCount;
                                bool okCount = int.TryParse(parts[2], out parsedSavedCount);
                                bool isConflict = parts[3] == "True";
                                string statusReason = parts[4];

                                this.Invoke(new Action(() =>
                                {
                                    lblCustomStatus.Text = statusReason;
                                    savedCount = okCount ? parsedSavedCount : savedCount;

                                    bool isVietnamese = IsVietnamese();
                                    lblCustomCount.Text = isVietnamese ?
                                        $"Đã ghi: {savedCount}/5" :
                                        $"Recorded: {savedCount}/5";

                                    if (eventName == "CONFLICT" || isConflict)
                                        lblCustomStatus.ForeColor = Color.Red;
                                    else if (eventName == "ERROR")
                                        lblCustomStatus.ForeColor = Color.OrangeRed;
                                    else
                                        lblCustomStatus.ForeColor = Color.DarkGreen;

                                    // ✅ MAIN CHANGE: Use CustomMessageBox and navigate to FormUserGestureCustom
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
                                                var customGestureForm = new FormUserGestureCustom(_homeForm);
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
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
            _homeForm.Show();
            this.Close();
        }
    }
}