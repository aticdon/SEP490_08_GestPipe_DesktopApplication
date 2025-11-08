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
        private bool receiving = false;
        private TcpClient statusClient;
        private NetworkStream statusStream;
        private Thread statusThread;
        private Process pythonProcess;


        private StringBuilder pythonErrorBuffer = new StringBuilder();
        private HomeUser _homeForm;
        public FormCustomGesture(HomeUser homeForm)
        {
            InitializeComponent();
            StartPythonProcess();
            pictureBoxCustom.SizeMode = PictureBoxSizeMode.Zoom; // Đảm bảo ảnh không bị crop
            btnStart.Click += btnStart_Click;
            _homeForm = homeForm;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string gestureName = txtGestureName.Text.Trim();
            if (string.IsNullOrEmpty(gestureName))
            {
                MessageBox.Show("Nhập tên gesture trước khi bắt đầu.");
                return;
            }
            Thread.Sleep(2000);
            SendGestureNameToPython(gestureName);
            StartReceivingCameraFrames(6001);
            StartReceivingStatus(6002);
        }

        private void SendGestureNameToPython(string gestureName)
        {
            try
            {
                using (var client = new TcpClient("127.0.0.1", 7000))
                using (var stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(gestureName);
                    stream.Write(data, 0, data.Length);
                    byte[] resp = new byte[32];
                    int len = stream.Read(resp, 0, resp.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Lỗi gửi tên gesture: " + ex.ToString());
            }
        }
        private void StartReceivingStatus(int port)
        {
            statusThread = new Thread(() =>
            {
                try
                {
                    statusClient = new TcpClient("127.0.0.1", port);
                    statusStream = statusClient.GetStream();
                    byte[] buffer = new byte[128];
                    while (true)
                    {
                        int received = statusStream.Read(buffer, 0, buffer.Length);
                        if (received > 0)
                        {
                            string text = Encoding.UTF8.GetString(buffer, 0, received);
                            var parts = text.Split('|');
                            if (parts.Length == 3)
                            {
                                this.Invoke(new Action(() => {
                                    lblState.Text = "State: " + parts[0];
                                    lblPose.Text = "Pose: " + parts[1];
                                    lblSaved.Text = "Saved: " + parts[2];
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Lỗi nhận status: " + ex.Message);
                    Console.WriteLine("Lỗi nhận status " + ex.Message);
                }
            });
            statusThread.IsBackground = true;
            statusThread.Start();
        }
        private void StartReceivingCameraFrames(int port)
        {
            receiving = true;
            cameraThread = new Thread(() =>
            {
                try
                {
                    cameraClient = new TcpClient("127.0.0.1", port);
                    cameraStream = cameraClient.GetStream();
                    Console.WriteLine("[C#] Đã kết nối tới Python camera server...");
                    while (receiving)
                    {
                        // Đọc độ dài frame
                        byte[] lengthBytes = new byte[4];
                        int read = 0;
                        while (read < 4)
                        {
                            int r = cameraStream.Read(lengthBytes, read, 4 - read);
                            if (r <= 0)
                            {
                                Console.WriteLine("[C#] Không đọc được độ dài frame!");
                                return;
                            }
                            read += r;
                        }
                        int length = BitConverter.ToInt32(lengthBytes.Reverse().ToArray(), 0);
                        Console.WriteLine($"[C#] Độ dài frame nhận: {length} bytes");
                        if (length < 1000 || length > 1000000)
                        {
                            Console.WriteLine("[C#] Cảnh báo: Độ dài frame bất thường!");
                        }

                        // Đọc dữ liệu ảnh
                        byte[] imageBytes = new byte[length];
                        read = 0;
                        while (read < length)
                        {
                            int r = cameraStream.Read(imageBytes, read, length - read);
                            if (r <= 0)
                            {
                                Console.WriteLine("[C#] Không đọc đủ dữ liệu ảnh!");
                                return;
                            }
                            read += r;
                        }
                        Console.WriteLine($"[C#] Số byte ảnh nhận đủ: {read} bytes");

                        // Decode ảnh
                        try
                        {
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                var img = Image.FromStream(ms);
                                Console.WriteLine("[C#] Decode ảnh thành công!");
                                this.Invoke(new Action(() =>
                                {
                                    //// Đảm bảo PictureBox vừa với ảnh
                                    //pictureBoxCustom.Width = img.Width;
                                    //pictureBoxCustom.Height = img.Height;
                                    //// Đảm bảo Form đủ lớn để chứa ảnh và controls phía trên
                                    //this.ClientSize = new Size(
                                    //    Math.Max(pictureBoxCustom.Width + 24, this.ClientSize.Width),
                                    //    pictureBoxCustom.Height + pictureBoxCustom.Top + 48 // cộng thêm chiều cao control trên
                                    //);
                                    pictureBoxCustom.Image = img;
                                }));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[C#] Lỗi decode ảnh: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[C#] Lỗi kết nối camera: " + ex.Message);
                }
            });
            cameraThread.IsBackground = true;
            cameraThread.Start();
        }
        private void StartPythonProcess()
        {
            try
            {
                //string pythonExePath = "python"; // hoặc @"C:\Users\THUCLTCE171961\AppData\Local\Programs\Python\Python39\python.exe"
                string pythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";
                string scriptFile = @"D:\Semester9\codepython\hybrid_realtime_pipeline\collect_data_hybrid.py";

                //Debug.WriteLine("Python exe path: " + pythonExePath);
                //Debug.WriteLine("Python script path: " + scriptFile);

                if (!File.Exists(scriptFile))
                {
                    //MessageBox.Show("Không tìm thấy file script python: " + scriptFile, "Python Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Debug.WriteLine("Không tìm thấy script python: " + scriptFile);
                    return;
                }

                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    //Debug.WriteLine("Python process đã chạy rồi.");
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
                //pythonProcess.Exited += (s, e) =>
                //{
                //    if (pythonErrorBuffer.Length > 0)
                //    {
                //        this.Invoke(new Action(() =>
                //        {
                //            MessageBox.Show(pythonErrorBuffer.ToString(), "PYTHON ERROR");
                //        }));
                //    }
                //};
                //Debug.WriteLine("Check python exe: " + File.Exists(pythonExePath));
                //Debug.WriteLine("Check script file: " + File.Exists(scriptFile));
                //Debug.WriteLine("WorkingDir: " + pythonProcess.StartInfo.WorkingDirectory);
                bool started = pythonProcess.Start();
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
                //Debug.WriteLine(File.Exists(pythonExePath) ? "Python.exe FOUND" : "Python.exe NOT FOUND");
                //Debug.WriteLine(File.Exists(scriptFile) ? "Script FOUND" : "Script NOT FOUND");
                //Debug.WriteLine(started
                //    ? $"Started Python process: {pythonExePath} {pythonProcess.StartInfo.Arguments}"
                //    : "Failed to start Python process.");
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Lỗi chạy Python: " + ex.Message, "Python Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("Lỗi chạy Python: " + ex.ToString());
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            receiving = false;
            cameraThread?.Abort();
            cameraStream?.Close();
            cameraClient?.Close();
            statusThread?.Abort();
            statusStream?.Close();
            statusClient?.Close();
            try
            {
                if (pythonProcess != null && !pythonProcess.HasExited)
                {
                    pythonProcess.Kill();
                }
            }
            catch { }
            base.OnFormClosing(e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _homeForm.Show();
            this.Close();
        }
    }
}