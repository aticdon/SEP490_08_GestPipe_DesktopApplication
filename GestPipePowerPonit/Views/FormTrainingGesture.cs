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
using System.Web.UI.HtmlControls;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class FormTrainingGesture : Form
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
        private FormUserGesture _userGestureForm;
        public string _currentUserId = Properties.Settings.Default.UserId;

        private int totalTrain = 0;
        private int correctTrain = 0;
        private string poseLabel = "";
        private string gestureName = "";// Nhận từ actionName khi Training
        private VectorData vectorData = null; // Nhận từ GestureDetail khi mở form
        private TrainingGestureService trainingGestureService = new TrainingGestureService();


        public FormTrainingGesture(HomeUser homeForm, FormUserGesture userGestureForm, string actionName, VectorData vectorData,string gestureName)
        {
            InitializeComponent();
            StartPythonProcess();
            InitCustomControls();
            _homeForm = homeForm;
            _userGestureForm = userGestureForm;
            this.gestureName = gestureName;
            this.poseLabel = actionName;
            this.vectorData = vectorData;
            //lblGestureName.Text = "Gesture Name: " + gestureName;
            ApplyLanguage();
        }
        private void InitCustomControls()
        {
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
                        MessageBox.Show("Gửi tên gesture lỗi: " + ex.Message);
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
                            this.Invoke(new Action(() => { picCamera.Image = img; }));
                        }
                    }
                }
                catch { }
            });
            cameraThread.IsBackground = true;
            cameraThread.Start();
        }
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
                    byte[] buffer = new byte[256];
                    while (true)
                    {
                        int received = statusStream.Read(buffer, 0, buffer.Length);
                        if (received > 0)
                        {
                            string text = Encoding.UTF8.GetString(buffer, 0, received);
                            // status: CORRECT|pose|correct|wrong|acc|reason
                            var parts = text.Split('|');
                            if (parts.Length >= 6)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    lblResult.Text = "✅ " + Properties.Resources.Lbl_LastResult + ": " + TranslateResult(parts[0]);
                                    lblPose.Text = "🎯 " + Properties.Resources.Lbl_PoseTarget + ": " + parts[1];
                                    lblCorrect.Text = "✅ " + Properties.Resources.Lbl_Result_Correct + ": " + parts[2];
                                    lblWrong.Text = "❌ " + Properties.Resources.Lbl_Result_Wrong + ": " + parts[3];
                                    lblAccuracy.Text = "📊 " + Properties.Resources.Lbl_Accuracy + ": " + parts[4] + "%";
                                    lblReason.Text = Properties.Resources.Lbl_Reason + ": " + parts[5];

                                    // Lưu lần cuối poseLabel để lưu ra DB
                                    poseLabel = parts[1];

                                    // Cập nhật biến số lần train/đúng
                                    int cTrain = 0, wTrain = 0;
                                    int.TryParse(parts[2], out cTrain);
                                    int.TryParse(parts[3], out wTrain);
                                    correctTrain = cTrain;
                                    totalTrain = cTrain + wTrain;
                                }));
                            }
                        }
                    }
                }
                catch { }
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
                //string pythonExePath = "python"; // hoặc @"C:\Users\THUCLTCE171961\AppData\Local\Programs\Python\Python39\python.exe"
                string pythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";
                string scriptFile = @"D:\Semester9\codepython\hybrid_realtime_pipeline\training_session_ml.py";

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
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        public void StartTrainingWithAction(string actionName)
        {
            //if (cboGesture.Items.Contains(actionName))
            //    cboGesture.SelectedItem = actionName;
            lblPose.Text = "Pose: " + actionName; // Hiện tên action lên giao diện
            SendPoseNameToPython(actionName);      // Gửi tên động tác qua python
            StartReceivingCameraFrames(6001);      // Bắt đầu lấy hình
            StartReceivingTrainingStatus(6002);    // Bắt đầu lấy trạng thái
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SaveTrainingResultToDb();
            this.Close();
            double accuracy = (totalTrain == 0) ? 0 : (double)correctTrain / totalTrain * 100.0;
            DateTime trainingDay = DateTime.Now;
            var resultForm = new FormTrainingResult(gestureName, 
                poseLabel, 
                accuracy, 
                trainingDay,
                _userGestureForm);
            resultForm.Show();
        }
        private void ApplyLanguage()
        {
            // Các label chính, caption, value
            lblGestureName.Text = Properties.Resources.Col_Name + ": " + gestureName;

            lblResult.Text = Properties.Resources.Lbl_LastResult + ":";
            lblPose.Text = Properties.Resources.Lbl_PoseTarget + ":";
            lblCorrect.Text = Properties.Resources.Lbl_Result_Correct + ":";
            lblWrong.Text = Properties.Resources.Lbl_Result_Wrong + ":";
            lblAccuracy.Text = Properties.Resources.Lbl_Accuracy + ":";
            lblReason.Text = Properties.Resources.Lbl_Reason + ":";

            // Các button
            btnHome.Text = Properties.Resources.Btn_Home;
            btnGestureControl.Text = Properties.Resources.Btn_GestureControl;
            btnVersion.Text = Properties.Resources.Btn_Version;
            btnInstruction.Text = Properties.Resources.Btn_Instruction;
            btnCustomeGesture.Text = Properties.Resources.Btn_CustomGesture;
            btnPresentation.Text = Properties.Resources.Btn_Present;
            btnEndTraining.Text = Properties.Resources.LblClose;
            // ... add thêm các caption, heading, tool tip nếu có

            // Nếu có các label phụ/placeholder, add dưới đây
            // lblTitle1.Text = Properties.Resources.Lbl_Title1;
            // lblTitle2.Text = Properties.Resources.Lbl_Title2;
        }

        private void guna2ControlBoxClose_Click(object sender, EventArgs e)
        {
            AppSettings.ExitAll();
        }
    }
}