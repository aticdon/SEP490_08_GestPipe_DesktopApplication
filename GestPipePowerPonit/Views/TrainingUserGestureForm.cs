using GestPipePowerPonit.I18n;
using GestPipePowerPonit.Models;
using GestPipePowerPonit.Services;
using System;
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
    public partial class TrainingUserGestureForm : Form
    {
        // Socket/thread references
        private TcpClient cameraClient;
        private NetworkStream cameraStream;
        private Thread cameraThread;

        private TcpClient statusClient;
        private NetworkStream statusStream;
        private Thread statusThread;

        private Process pythonProcess;
        private StringBuilder pythonErrorBuffer = new StringBuilder();

        // Form references
        private HomeUser _homeForm;
        private ListDefaultGestureForm _defaultGesture;

        // User and training data
        public string _currentUserId = Properties.Settings.Default.UserId;
        private int totalTrain = 0;
        private int correctTrain = 0;
        private string poseLabel = "";
        private string gestureName = "";
        private VectorData vectorData = null;
        private TrainingGestureService trainingGestureService = new TrainingGestureService();

        // UI state
        private int spinnerAngle = 0;
        private bool firstFrameReceived = false;
        private Label lblModelType; // ✅ NEW: Display model type

        public TrainingUserGestureForm(HomeUser homeForm, ListDefaultGestureForm defaultGesture,
                                      string actionName, VectorData vectorData, string gestureName)
        {
            InitializeComponent();

            // ✅ VALIDATE USER_ID
            if (string.IsNullOrEmpty(Properties.Settings.Default.UserId))
            {
                Debug.WriteLine("⚠️ Warning: UserId is empty, will use general models");
                _currentUserId = ""; // Will fallback to general models in Python
            }
            else
            {
                _currentUserId = Properties.Settings.Default.UserId;
                Debug.WriteLine($"✅ User ID: {_currentUserId}");
            }

            StartPythonProcess();
            InitCustomControls();

            _homeForm = homeForm;
            _defaultGesture = defaultGesture;
            this.gestureName = gestureName;
            this.poseLabel = actionName;
            this.vectorData = vectorData;

            ApplyLanguage();

            Debug.WriteLine($"📋 Training Session Info:");
            Debug.WriteLine($"   - Gesture: {gestureName}");
            Debug.WriteLine($"   - Action: {actionName}");
            Debug.WriteLine($"   - User: {_currentUserId}");
        }

        private void InitCustomControls()
        {
            // ✅ ADD: Label to display model type
            lblModelType = new Label
            {
                Name = "lblModelType",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(125, 137, 149),
                Location = new Point(707, 206),
                Text = "Model: Loading..."
            };

            pnlMain.Controls.Add(lblModelType);
            lblModelType.BringToFront();
        }

        public void FormTrainingGesture_Load(object sender, EventArgs e)
        {
            btnEndTraining.Enabled = false;
        }

        #region Loading Spinner Animation

        private void SpinnerTimer_Tick(object sender, EventArgs e)
        {
            spinnerAngle += 15;
            if (spinnerAngle >= 360) spinnerAngle = 0;
            DrawSpinner();
        }

        private void DrawSpinner()
        {
            Bitmap oldBitmap = loadingSpinner.Image as Bitmap;

            try
            {
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
                        double angle = (spinnerAngle + i * 45) * Math.PI / 180;
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
                oldBitmap?.Dispose();
            }
            catch
            {
                oldBitmap?.Dispose();
            }
        }

        /// <summary>
        /// Update loading text based on language
        /// </summary>
        private void UpdateLoadingText()
        {
            string loadingText = GetLocalizedText("loading_training",
                "Starting custom training session...\nPlease wait...",
                "Đang khởi động phiên huấn luyện tùy chỉnh...\nVui lòng đợi...");

            loadingLabel.Text = loadingText;

            loadingLabel.Location = new Point(
                (loadingPanel.Width - loadingLabel.Width) / 2,
                loadingSpinner.Bottom + 20
            );
        }

        /// <summary>
        /// Show loading overlay
        /// </summary>
        private void ShowLoading()
        {
            UpdateLoadingText();
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            spinnerTimer.Start();
            firstFrameReceived = false;
        }

        /// <summary>
        /// Hide loading overlay
        /// </summary>
        private void HideLoading()
        {
            loadingPanel.Visible = false;
            spinnerTimer.Stop();
        }

        #endregion

        #region Localization Helpers

        /// <summary>
        /// Get localized text based on current language
        /// </summary>
        private string GetLocalizedText(string key, string englishText, string vietnameseText)
        {
            return IsVietnamese() ? vietnameseText : englishText;
        }

        /// <summary>
        /// Check if current language is Vietnamese
        /// </summary>
        private bool IsVietnamese()
        {
            try
            {
                return GestPipePowerPonit.CultureManager.CurrentCultureCode.Contains("vi") ||
                       AppSettings.CurrentLanguage == "VN";
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Translate result from Python (CORRECT/WRONG)
        /// </summary>
        private string TranslateResult(string value)
        {
            if (value == "CORRECT") return Properties.Resources.Lbl_Result_Correct;
            if (value == "WRONG") return Properties.Resources.Lbl_Result_Wrong;
            return value;
        }

        #endregion

        #region Socket Communication

        /// <summary>
        /// ✅ NEW: Send pose name AND user_id to Python
        /// Format: pose_name|user_id
        /// </summary>
        private void SendPoseAndUserToPython(string poseName, string userId)
        {
            int retry = 0;
            int maxRetries = 10;

            while (retry < maxRetries)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        client.ReceiveTimeout = 5000;
                        client.SendTimeout = 5000;

                        Debug.WriteLine($"[Attempt {retry + 1}/{maxRetries}] Connecting to Python on port 7000...");
                        client.Connect("127.0.0.1", 7000);

                        using (var stream = client.GetStream())
                        {
                            // ✅ SEND NEW FORMAT: pose_name|user_id
                            string message = $"{poseName}|{userId}";
                            byte[] data = Encoding.UTF8.GetBytes(message);

                            stream.Write(data, 0, data.Length);
                            Debug.WriteLine($"✅ Sent to Python: {message}");

                            // Wait for response
                            byte[] resp = new byte[32];
                            int len = stream.Read(resp, 0, resp.Length);
                            string response = Encoding.UTF8.GetString(resp, 0, len);

                            Debug.WriteLine($"✅ Python response: {response}");
                        }

                        return; // Success
                    }
                }
                catch (Exception ex)
                {
                    retry++;
                    Debug.WriteLine($"❌ Connection failed (attempt {retry}/{maxRetries}): {ex.Message}");

                    if (retry >= maxRetries)
                    {
                        string errorMsg = GetLocalizedText("python_connection_error",
                            $"Failed to connect to Python server after {maxRetries} attempts.\n\nError: {ex.Message}",
                            $"Không thể kết nối tới Python server sau {maxRetries} lần thử.\n\nLỗi: {ex.Message}");

                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show(errorMsg, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }));

                        return;
                    }

                    Thread.Sleep(500);
                }
            }
        }

        /// <summary>
        /// ✅ IMPROVED: Start receiving camera frames with retry logic
        /// </summary>
        private void StartReceivingCameraFrames(int port)
        {
            if (cameraThread != null && cameraThread.IsAlive)
            {
                Debug.WriteLine("⚠️ Camera thread already running");
                return;
            }

            cameraThread = new Thread(() =>
            {
                int reconnectAttempts = 0;
                const int maxReconnects = 3;

                while (reconnectAttempts < maxReconnects)
                {
                    try
                    {
                        Debug.WriteLine($"[Camera] Connecting to port {port}... (attempt {reconnectAttempts + 1})");

                        cameraClient = new TcpClient("127.0.0.1", port);
                        cameraStream = cameraClient.GetStream();

                        Debug.WriteLine("✅ Camera connected successfully!");
                        reconnectAttempts = 0;

                        while (true)
                        {
                            // Read frame length (4 bytes, big-endian)
                            byte[] lenBuf = new byte[4];
                            int read = 0;
                            while (read < 4)
                            {
                                int r = cameraStream.Read(lenBuf, read, 4 - read);
                                if (r <= 0)
                                {
                                    Debug.WriteLine("❌ Camera stream closed by server");
                                    throw new Exception("Stream closed");
                                }
                                read += r;
                            }

                            int length = BitConverter.ToInt32(lenBuf.Reverse().ToArray(), 0);

                            if (length < 1000 || length > 2000000)
                            {
                                Debug.WriteLine($"⚠️ Invalid frame size: {length}");
                                continue;
                            }

                            // Read frame data
                            byte[] imgBuf = new byte[length];
                            read = 0;
                            while (read < length)
                            {
                                int r = cameraStream.Read(imgBuf, read, length - read);
                                if (r <= 0)
                                {
                                    Debug.WriteLine("❌ Camera stream closed while reading frame");
                                    throw new Exception("Stream closed");
                                }
                                read += r;
                            }

                            // Decode and display image
                            using (var ms = new MemoryStream(imgBuf))
                            {
                                try
                                {
                                    var img = Image.FromStream(ms);

                                    this.Invoke(new Action(() =>
                                    {
                                        var oldImage = picCamera.Image;
                                        picCamera.Image = img;
                                        oldImage?.Dispose();

                                        if (!firstFrameReceived)
                                        {
                                            firstFrameReceived = true;
                                            HideLoading();
                                            Debug.WriteLine("✅ First frame received!");
                                        }
                                    }));
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"❌ Failed to decode frame: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        reconnectAttempts++;
                        Debug.WriteLine($"❌ Camera connection error: {ex.Message}");

                        try
                        {
                            cameraStream?.Close();
                            cameraClient?.Close();
                        }
                        catch { }

                        if (reconnectAttempts < maxReconnects)
                        {
                            Debug.WriteLine($"🔄 Reconnecting camera... ({reconnectAttempts}/{maxReconnects})");
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Debug.WriteLine("❌ Camera connection failed after max retries");

                            this.Invoke(new Action(() =>
                            {
                                HideLoading();

                                string errorMsg = GetLocalizedText("camera_connection_lost",
                                    "Camera connection lost. Please restart training.",
                                    "Mất kết nối camera. Vui lòng khởi động lại.");

                                MessageBox.Show(errorMsg, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));

                            break;
                        }
                    }
                }
            })
            {
                IsBackground = true,
                Name = "CameraReceiverThread"
            };

            cameraThread.Start();
        }

        /// <summary>
        /// Start receiving training status with bilingual support
        /// </summary>
        private void StartReceivingTrainingStatus(int port)
        {
            if (statusThread != null && statusThread.IsAlive)
                return;

            statusThread = new Thread(() =>
            {
                try
                {
                    statusClient = new TcpClient("127.0.0.1", port);
                    statusStream = statusClient.GetStream();
                    byte[] buffer = new byte[1024];

                    while (true)
                    {
                        int received = statusStream.Read(buffer, 0, buffer.Length);
                        if (received > 0)
                        {
                            string text = Encoding.UTF8.GetString(buffer, 0, received);
                            Debug.WriteLine($"[TrainingGesture] Status received: {text}");
                            var parts = text.Split('|');

                            if (parts.Length >= 6)
                            {
                                string result = parts[0];
                                string pose = parts[1];
                                string correct = parts[2];
                                string wrong = parts[3];
                                string accuracy = parts[4];

                                string reasonMessage;

                                if (parts.Length >= 7)
                                {
                                    string englishReason = parts[5];
                                    string vietnameseReason = parts[6];
                                    reasonMessage = IsVietnamese() ? vietnameseReason : englishReason;
                                }
                                else
                                {
                                    reasonMessage = parts[5];
                                }

                                this.Invoke(new Action(() =>
                                {
                                    lblResult.Text = "✅ " + Properties.Resources.Lbl_LastResult + ": " + TranslateResult(result);
                                    lblPose.Text = "🎯 " + Properties.Resources.Lbl_PoseTarget + ": " + pose;
                                    lblCorrect.Text = "✅ " + Properties.Resources.Lbl_Result_Correct + ": " + correct;
                                    lblWrong.Text = "❌ " + Properties.Resources.Lbl_Result_Wrong + ": " + wrong;
                                    lblAccuracy.Text = "📊 " + Properties.Resources.Lbl_Accuracy + ": " + accuracy + "%";
                                    lblReason.Text = Properties.Resources.Lbl_Reason + ": " + reasonMessage;

                                    poseLabel = pose;

                                    int cTrain = 0, wTrain = 0;
                                    int.TryParse(correct, out cTrain);
                                    int.TryParse(wrong, out wTrain);
                                    correctTrain = cTrain;
                                    totalTrain = cTrain + wTrain;
                                }));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"[TrainingGesture] Error in status thread: {e.Message}");
                }
            })
            {
                IsBackground = true,
                Name = "StatusReceiverThread"
            };

            statusThread.Start();
        }

        #endregion

        #region Python Process Management

        /// <summary>
        /// ✅ UPDATED: Start custom training Python script
        /// </summary>
        private void StartPythonProcess()
        {
            try
            {
                string pythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";

                // ✅ CHANGE: Use custom_training_session_socket.py
                string scriptFile = @"D:\Semester9\codepython\hybrid_realtime_pipeline\code\practice_session.py";

                if (!File.Exists(scriptFile))
                {
                    MessageBox.Show($"Python script not found: {scriptFile}", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    Debug.WriteLine("Python process already running");
                    return;
                }

                pythonProcess = new Process();
                pythonProcess.StartInfo.FileName = pythonExePath;
                pythonProcess.StartInfo.Arguments = $"\"{scriptFile}\"";
                pythonProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptFile);
                pythonProcess.StartInfo.UseShellExecute = false;
                pythonProcess.StartInfo.RedirectStandardOutput = true;
                pythonProcess.StartInfo.RedirectStandardError = true;
                pythonProcess.StartInfo.CreateNoWindow = true;

                pythonProcess.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.WriteLine($"[Python Output] {e.Data}");
                        File.AppendAllText("python_custom_stdout.log", e.Data + Environment.NewLine);
                    }
                };

                pythonProcess.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        pythonErrorBuffer.AppendLine(e.Data);
                        Debug.WriteLine($"[Python Error] {e.Data}");
                        File.AppendAllText("python_custom_stderr.log", e.Data + Environment.NewLine);
                    }
                };

                pythonProcess.EnableRaisingEvents = true;
                bool started = pythonProcess.Start();

                if (started)
                {
                    pythonProcess.BeginOutputReadLine();
                    pythonProcess.BeginErrorReadLine();
                    Debug.WriteLine("✅ Custom Python training process started successfully");
                }
                else
                {
                    MessageBox.Show("Failed to start Python process", "Error",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error starting Python: {ex}");
                MessageBox.Show($"Error starting Python process: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Training Session Control

        /// <summary>
        /// ✅ UPDATED: Start training with action and user_id
        /// </summary>
        public async void StartTrainingWithAction(string actionName)
        {
            try
            {
                ShowLoading();

                lblPose.Text = "🎯 Pose: " + actionName;

                // ✅ Display model type
                string modelInfo = string.IsNullOrEmpty(_currentUserId)
                    ? "Using: General Models"
                    : $"Using: {_currentUserId}'s Custom Models";

                lblModelType.Text = modelInfo;
                Debug.WriteLine($"[Training] {modelInfo}");

                await Task.Run(() =>
                {
                    // ✅ SEND POSE NAME AND USER_ID
                    SendPoseAndUserToPython(actionName, _currentUserId);
                    Thread.Sleep(2000);
                });

                StartReceivingCameraFrames(6001);
                StartReceivingTrainingStatus(6002);

                await WaitForCameraConnectionAsync();
            }
            catch (Exception ex)
            {
                HideLoading();
                Debug.WriteLine($"[StartTrainingWithAction] Error: {ex.Message}");

                string errorMsg = GetLocalizedText("training_start_error",
                    $"Failed to start training session.\n\nError: {ex.Message}",
                    $"Không thể bắt đầu phiên huấn luyện.\n\nLỗi: {ex.Message}");

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Wait for camera connection with timeout
        /// </summary>
        private async Task WaitForCameraConnectionAsync()
        {
            int timeout = 10000;
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

            HideLoading();
            string timeoutMsg = GetLocalizedText("camera_timeout",
                "Camera connection timeout, but continuing...",
                "Kết nối camera timeout, nhưng vẫn tiếp tục...");

            lblResult.Text = timeoutMsg;
            lblResult.ForeColor = Color.Orange;
        }

        #endregion

        #region Database Operations

        /// <summary>
        /// Save training results to database
        /// </summary>
        private async void SaveTrainingResultToDb()
        {
            var result = new TrainingGesture
            {
                Id = "",
                UserId = this._currentUserId,
                PoseLabel = poseLabel,
                TotalTrain = totalTrain,
                CorrectTrain = correctTrain,
                Accuracy = (totalTrain == 0) ? 0 : (double)correctTrain / totalTrain * 100.0,
                VectorData = vectorData,
                CreateAt = DateTime.Now
            };

            try
            {
                bool ok = await trainingGestureService.SaveTrainingGestureAsync(result);
                Debug.WriteLine($"✅ Training result saved: {ok}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error saving training result: {ex.Message}");
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        #endregion

        #region UI Event Handlers

        /// <summary>
        /// Home button click - return to home form
        /// </summary>
        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }

        /// <summary>
        /// End training button click
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (totalTrain >= 5)
            {
                btnEndTraining.Enabled = true;
                SaveTrainingResultToDb();
                this.Close();

                double accuracy = (totalTrain == 0) ? 0 : (double)correctTrain / totalTrain * 100.0;
                DateTime trainingDay = DateTime.Now;

                var resultForm = new TraingResultForm(gestureName,
                    poseLabel,
                    accuracy,
                    trainingDay,
                    _defaultGesture);

                resultForm.Show();
            }
            else
            {
                string msg = GetLocalizedText("min_training_required",
                    "Please complete at least 5 training attempts.",
                    "Vui lòng hoàn thành ít nhất 5 lần huấn luyện.");

                MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }

        #endregion

        #region Form Lifecycle

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                Debug.WriteLine("🛑 Closing training form...");

                // Stop spinner
                spinnerTimer?.Stop();
                spinnerTimer?.Dispose();

                // Close camera connection
                try
                {
                    cameraStream?.Close();
                    cameraClient?.Close();

                    if (cameraThread != null && cameraThread.IsAlive)
                    {
                        cameraThread.Join(1000);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error closing camera: {ex.Message}");
                }

                // Close status connection
                try
                {
                    statusStream?.Close();
                    statusClient?.Close();

                    if (statusThread != null && statusThread.IsAlive)
                    {
                        statusThread.Join(1000);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error closing status: {ex.Message}");
                }

                // Kill Python process
                try
                {
                    if (pythonProcess != null && !pythonProcess.HasExited)
                    {
                        Debug.WriteLine("🔪 Killing Python process...");
                        pythonProcess.Kill();
                        pythonProcess.WaitForExit(2000);
                        pythonProcess.Dispose();
                        Debug.WriteLine("✅ Python process terminated");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error killing Python: {ex.Message}");
                }

                // Dispose spinner image
                if (loadingSpinner.Image != null)
                {
                    loadingSpinner.Image.Dispose();
                }

                Debug.WriteLine("✅ Training form closed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in OnFormClosing: {ex}");
            }

            base.OnFormClosing(e);
        }

        private void ApplyLanguage()
        {
            ResourceHelper.SetCulture(CultureManager.CurrentCultureCode, this);

            lblGestureName.Text = Properties.Resources.Col_Name + ": " + gestureName;
            lblResult.Text = Properties.Resources.Lbl_LastResult + ":";
            lblPose.Text = Properties.Resources.Lbl_PoseTarget + ":";
            lblCorrect.Text = Properties.Resources.Lbl_Result_Correct + ":";
            lblWrong.Text = Properties.Resources.Lbl_Result_Wrong + ":";
            lblAccuracy.Text = Properties.Resources.Lbl_Accuracy + ":";
            lblReason.Text = Properties.Resources.Lbl_Reason + ":";

            btnPresentation.Text = Properties.Resources.Btn_Present;
            btnEndTraining.Text = Properties.Resources.LblClose;
        }

        #endregion
    }
}