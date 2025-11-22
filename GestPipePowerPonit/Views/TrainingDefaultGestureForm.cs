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
using System.Web.UI.HtmlControls;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class TrainingDefaultGestureForm : Form
    {
        // Socket/thread tham chiếu
        private TcpClient cameraClient;
        private NetworkStream cameraStream;
        private Thread cameraThread;

        private TcpClient statusClient;
        private NetworkStream statusStream;
        private Thread statusThread;
        private Process pythonProcess;
        private StringBuilder pythonErrorBuffer = new StringBuilder();
        private HomeUser _homeForm;
        //private FormUserGesture _userGestureForm;
        private ListDefaultGestureForm _defaultGesture;
        public string _currentUserId = Properties.Settings.Default.UserId;

        private int totalTrain = 0;
        private int correctTrain = 0;
        private string poseLabel = "";
        private string gestureName = "";// Nhận từ actionName khi Training
        private VectorData vectorData = null; // Nhận từ GestureDetail khi mở form
        private TrainingGestureService trainingGestureService = new TrainingGestureService();
        private int spinnerAngle = 0;
        private bool firstFrameReceived = false;


        //public FormTrainingGesture(HomeUser homeForm, FormUserGesture userGestureForm, string actionName, VectorData vectorData, string gestureName)
        public TrainingDefaultGestureForm(HomeUser homeForm, ListDefaultGestureForm defaultGesture, string actionName, VectorData vectorData,  string gestureName)
        {
            InitializeComponent();
            StartPythonProcess();
            InitCustomControls();
            _homeForm = homeForm;
            _defaultGesture = defaultGesture;
            this.gestureName = gestureName;
            this.poseLabel = actionName;
            this.vectorData = vectorData;
            ApplyLanguage();
        }

        private void InitCustomControls()
        {
        }
        public void FormTrainingGesture_Load(object sender, EventArgs e)
        {
            btnEndTraining.Enabled = false;
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
            string loadingText = GetLocalizedText("loading_training",
                "Starting training session...\nPlease wait...",
                "Đang khởi động phiên huấn luyện...\nVui lòng đợi...");

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

        // ✅ THÊM: Helper method for localized text
        private string GetLocalizedText(string key, string englishText, string vietnameseText)
        {
            return IsVietnamese() ? vietnameseText : englishText;
        }

        // ✅ NEW: Helper method for getting current language status
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

        private void SendPoseNameToPython(string poseName)
        {
            int retry = 0;
            while (retry < 10)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect("127.0.0.1", 7000); // thử connect
                        using (var stream = client.GetStream())
                        {
                            byte[] data = Encoding.UTF8.GetBytes(poseName);
                            stream.Write(data, 0, data.Length);
                            byte[] resp = new byte[32];
                            int len = stream.Read(resp, 0, resp.Length);
                        }
                        return; // thành công thì thoát luôn
                    }
                }
                catch (Exception ex)
                {
                    if (retry == 9)
                        Console.WriteLine("Gửi tên gesture lỗi: " + ex.Message);
                    Thread.Sleep(500);  // chờ 0.5s rồi thử lại
                    retry++;
                }
            }
        }

        private void StartReceivingCameraFrames(int port)
        {
            if (cameraThread != null && cameraThread.IsAlive)
                return;
            cameraThread = new Thread(() =>
            {
                try
                {
                    cameraClient = new TcpClient("127.0.0.1", port);
                    cameraStream = cameraClient.GetStream();
                    while (true)
                    {
                        byte[] lenBuf = new byte[4];
                        int read = 0;
                        while (read < 4)
                        {
                            int r = cameraStream.Read(lenBuf, read, 4 - read);
                            if (r <= 0) return;
                            read += r;
                        }
                        int length = BitConverter.ToInt32(lenBuf.Reverse().ToArray(), 0);
                        if (length < 1000 || length > 1000000) continue;
                        byte[] imgBuf = new byte[length];
                        read = 0;
                        while (read < length)
                        {
                            int r = cameraStream.Read(imgBuf, read, length - read);
                            if (r <= 0) return;
                            read += r;
                        }
                        using (var ms = new System.IO.MemoryStream(imgBuf))
                        {
                            var img = Image.FromStream(ms);
                            //this.Invoke(new Action(() => { picCamera.Image = img; }));
                            this.Invoke(new Action(() =>
                            {
                                picCamera.Image = img;

                                // ✅ Ẩn loading khi nhận frame đầu tiên
                                if (!firstFrameReceived)
                                {
                                    firstFrameReceived = true;
                                    HideLoading();
                                }
                            }));
                        }
                    }
                }
                catch { }
            });
            cameraThread.IsBackground = true;
            cameraThread.Start();
        }

        // ✅ UPDATED: Enhanced to handle bilingual training status messages
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
                    byte[] buffer = new byte[1024]; // ✅ Increased buffer size for bilingual messages
                    while (true)
                    {
                        int received = statusStream.Read(buffer, 0, buffer.Length);
                        if (received > 0)
                        {
                            string text = Encoding.UTF8.GetString(buffer, 0, received);
                            Debug.WriteLine($"[TrainingGesture] Status received: {text}");
                            var parts = text.Split('|');

                            // ✅ NEW: Handle both old format (6 parts) and new bilingual format (7 parts)
                            if (parts.Length >= 6)
                            {
                                string result = parts[0];        // CORRECT/WRONG
                                string pose = parts[1];          // pose name
                                string correct = parts[2];       // correct count
                                string wrong = parts[3];         // wrong count
                                string accuracy = parts[4];      // accuracy percentage

                                string reasonMessage;

                                if (parts.Length >= 7)
                                {
                                    // ✅ NEW FORMAT: Use appropriate language message
                                    string englishReason = parts[5];
                                    string vietnameseReason = parts[6];
                                    reasonMessage = IsVietnamese() ? vietnameseReason : englishReason;
                                }
                                else
                                {
                                    // ✅ OLD FORMAT: Use single reason (backward compatibility)
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

                                    // Lưu lần cuối poseLabel để lưu ra DB
                                    poseLabel = pose;

                                    // Cập nhật biến số lần train/đúng
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
            });
            statusThread.IsBackground = true;
            statusThread.Start();
        }

        // Chuyển result trả về từ Python sang text song ngữ
        string TranslateResult(String value)
        {
            if (value == "CORRECT") return Properties.Resources.Lbl_Result_Correct; // VD: "Correct"|"Đúng"
            if (value == "WRONG") return Properties.Resources.Lbl_Result_Wrong; // VD: "Wrong"|"Sai"
            return value;
        }

        private void StartPythonProcess()
        {
            try
            {
                string pythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";
                string userFolder = $"user_{_currentUserId}";
                // ✅ CHANGE: Use custom_training_session_socket.py
                string scriptFile = $@"D:\Semester9\codepython\hybrid_realtime_pipeline\code\{userFolder}\training_session_ml.py";
                if (!File.Exists(scriptFile))
                {
                    return;
                }

                if (pythonProcess != null && !pythonProcess.HasExited)
                {
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
                        File.AppendAllText("python_stdout.log", e.Data + Environment.NewLine);
                };
                pythonProcess.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        pythonErrorBuffer.AppendLine(e.Data);
                        File.AppendAllText("python_stderr.log", e.Data + Environment.NewLine);
                    }
                };

                pythonProcess.EnableRaisingEvents = true;
                bool started = pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi chạy Python: " + ex.ToString());
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                spinnerTimer?.Stop();
                cameraThread?.Abort();
                cameraStream?.Close();
                cameraClient?.Close();
                statusThread?.Abort();
                statusStream?.Close();
                statusClient?.Close();
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    pythonProcess.Kill();
                    pythonProcess.WaitForExit();
                }
            }
            catch { }
            base.OnFormClosing(e);
        }

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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        public async void StartTrainingWithAction(string actionName)
        {
            try
            {
                // ✅ Hiển thị loading
                ShowLoading();

                lblPose.Text = "Pose: " + actionName;

                // ✅ Chạy setup trong background
                await Task.Run(() =>
                {
                    SendPoseNameToPython(actionName);
                    Thread.Sleep(2000); // Đợi Python setup
                });

                // ✅ Bắt đầu nhận camera và status
                StartReceivingCameraFrames(6001);
                StartReceivingTrainingStatus(6002);

                // ✅ Đợi camera kết nối
                await WaitForCameraConnectionAsync();
            }
            catch (Exception ex)
            {
                HideLoading();
                Debug.WriteLine($"[StartTrainingWithAction] Error: {ex.Message}");
            }
        }
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

            lblResult.Text = timeoutMsg;
            lblResult.ForeColor = Color.Orange;
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(totalTrain >= 5)
            {
                btnEndTraining.Enabled = true;
                SaveTrainingResultToDb();
                
                double accuracy = (totalTrain == 0) ? 0 : (double)correctTrain / totalTrain * 100.0;
                DateTime trainingDay = DateTime.Now;
                _defaultGesture.Show();
                var resultForm = new TraingResultForm(gestureName,
                    poseLabel,
                    accuracy,
                    trainingDay,
                    //_userGestureForm);
                    _defaultGesture);
                //resultForm.Show();
                {
                    // form result nằm giữa form list
                    resultForm.StartPosition = FormStartPosition.CenterParent;

                    // CHÍNH ĐIỂM NÀY: modal, block tương tác list
                    resultForm.ShowDialog(_defaultGesture);
                }
                this.Close();
            }
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

            // Các button
            btnPresentation.Text = Properties.Resources.Btn_Present;
            btnEndTraining.Text = Properties.Resources.LblClose;
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}