using System;
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

        private int savedCount = 0;
        private const int CUSTOM_MAX = 5;

        public FormCustomGesture(HomeUser homeForm, string userName, string poseLabel)
        {
            InitializeComponent();
            _homeForm = homeForm;
            this.userName = userName;
            this.poseLabel = poseLabel;

            this.Load += FormCustomGesture_Load;
        }

        private void FormCustomGesture_Load(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine($"[CustomGesture] Form loaded. User: {userName}, Pose: {poseLabel}");
                StartPythonProcess();

                SendUserAndPoseToPython(userName, poseLabel);

                StartReceivingCameraFrames(6001);
                StartReceivingCustomStatus(6002);

                lblCustomInfo.Text = $"User: {userName}\r\nPose: {poseLabel}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khởi tạo CustomGesture: " + ex.Message);
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
            MessageBox.Show("Không thể gửi username/pose sang Python server sau 10 lần thử.");
        }
        private void StartPythonProcess()
        {
            try
            {
                string pythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";
                string scriptFile = @"D:\Semester9\codepython\hybrid_realtime_pipeline\code\collect_data_update.py";

                if (!File.Exists(scriptFile))
                {
                    MessageBox.Show("Không tìm thấy file Python script: " + scriptFile);
                    return;
                }

                if (pythonProcess != null && !pythonProcess.HasExited) return;

                pythonProcess = new Process();
                pythonProcess.StartInfo.FileName = pythonExePath;
                pythonProcess.StartInfo.Arguments = $"\"{scriptFile}\"";

                // ✅ ĐẢM BẢO WORKING DIRECTORY ĐÚNG
                string scriptDirectory = Path.GetDirectoryName(scriptFile);
                pythonProcess.StartInfo.WorkingDirectory = scriptDirectory;

                pythonProcess.StartInfo.UseShellExecute = false;
                pythonProcess.StartInfo.RedirectStandardOutput = true;
                pythonProcess.StartInfo.RedirectStandardError = true;
                pythonProcess.StartInfo.CreateNoWindow = true;
                pythonProcess.EnableRaisingEvents = true;

                // ✅ LOG ĐỂ DEBUG
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
                MessageBox.Show("Lỗi khi mở Python script: " + ex.Message);
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
                            // Event|Pose|SavedCount|Conflict|Reason
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
                                    lblCustomCount.Text = $"Đã ghi: {savedCount}/5";
                                    // Optionally: Show event type, show conflict warning, etc.
                                    if (eventName == "CONFLICT" || isConflict)
                                        lblCustomStatus.ForeColor = Color.Red;
                                    else if (eventName == "ERROR")
                                        lblCustomStatus.ForeColor = Color.OrangeRed;
                                    else
                                        lblCustomStatus.ForeColor = Color.DarkGreen;

                                    if (eventName == "FINISH")
                                    {
                                        MessageBox.Show($"Hoàn thành lưu file!\n{statusReason}", "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        this.Close();
                                    }
                                    else if (savedCount >= CUSTOM_MAX)
                                    {
                                        
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
                // ✅ ĐÓNG CONNECTIONS TRƯỚC ĐỂ PYTHON BIẾT VÀ THOÁT GRACEFULLY
                Debug.WriteLine("[CustomGesture] Closing connections to signal Python to exit...");

                cameraThread?.Abort();
                cameraStream?.Close();
                cameraClient?.Close();

                statusThread?.Abort();
                statusStream?.Close();
                statusClient?.Close();

                // ✅ CHỜ PYTHON TỰ THOÁT VÀ LƯU FILE (QUAN TRỌNG!)
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    Debug.WriteLine("[CustomGesture] Waiting for Python to save files and exit gracefully...");

                    // Chờ Python tự thoát khi detect connection lost
                    bool exitedGracefully = pythonProcess.WaitForExit(10000); // 10 giây

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