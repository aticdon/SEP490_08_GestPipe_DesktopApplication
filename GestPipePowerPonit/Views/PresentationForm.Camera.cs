using GestPipePowerPonit.Views;
using System;
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
    public partial class PresentationForm : Form
    {

        private void StartCameraReceiver(int port = 6000)
        {
            try
            {
                cameraListener = new TcpListener(System.Net.IPAddress.Any, port);
                cameraListener.Start();
                cameraRunning = true;

                cameraThread = new Thread(() =>
                {
                    try
                    {
                        Debug.WriteLine($"[Camera] Waiting for connection on port {port}...");

                        using (TcpClient client = cameraListener.AcceptTcpClient())
                        using (NetworkStream ns = client.GetStream())
                        {
                            Debug.WriteLine("[Camera] Client connected!");

                            while (cameraRunning)
                            {
                                byte[] lengthBytes = new byte[4];
                                int bytesRead = 0;

                                while (bytesRead < 4)
                                {
                                    int r = ns.Read(lengthBytes, bytesRead, 4 - bytesRead);
                                    if (r <= 0)
                                    {
                                        Debug.WriteLine("[Camera] Connection closed while reading length");
                                        return;
                                    }
                                    bytesRead += r;
                                }

                                int length = System.BitConverter.ToInt32(lengthBytes.Reverse().ToArray(), 0);

                                // Validate frame size
                                if (length < 1000 || length > 10000000)
                                {
                                    Debug.WriteLine($"[Camera] Invalid frame size: {length}");
                                    continue;
                                }

                                byte[] imageBytes = new byte[length];
                                int read = 0;

                                while (read < length)
                                {
                                    int r = ns.Read(imageBytes, read, length - read);
                                    if (r <= 0)
                                    {
                                        Debug.WriteLine("[Camera] Connection closed while reading image");
                                        return;
                                    }
                                    read += r;
                                }

                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    try
                                    {
                                        var img = Image.FromStream(ms);
                                        img.RotateFlip(RotateFlipType.RotateNoneFlipX);

                                        this.Invoke(new Action(() =>
                                        {
                                            var oldImage = pictureBoxCamera.Image;
                                            pictureBoxCamera.Image = img;
                                            oldImage?.Dispose();

                                            // ✅ ĐÁNh DẤU ĐÃ NHẬN FRAME ĐẦU TIÊN
                                            if (!firstCameraFrameReceived)
                                            {
                                                firstCameraFrameReceived = true;
                                                Debug.WriteLine("✅ First camera frame received!");
                                            }
                                        }));

                                        //Debug.WriteLine($"[Camera] Frame displayed ({length} bytes)");
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"[Camera] Error decoding frame: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[Camera] Thread error: {ex.Message}");
                    }
                    finally
                    {
                        Debug.WriteLine("[Camera] Thread exited");
                    }
                });

                cameraThread.IsBackground = true;
                cameraThread.Start();

                Debug.WriteLine($"[Camera] Receiver thread started on port {port}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Camera] StartCameraReceiver error: {ex.Message}");
            }
        }
        private void StopCameraReceiver()
        {
            Debug.WriteLine("[Camera] Stopping camera receiver...");

            cameraRunning = false;
            firstCameraFrameReceived = false; // ✅ Reset flag

            try
            {
                cameraListener?.Stop();

                if (cameraThread != null && cameraThread.IsAlive)
                {
                    if (!cameraThread.Join(2000)) // Chờ 2 giây
                    {
                        Debug.WriteLine("[Camera] Thread did not stop gracefully, aborting...");
                        cameraThread.Abort();
                    }
                }

                Debug.WriteLine("[Camera] Camera receiver stopped");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Camera] Error stopping camera: {ex.Message}");
            }
        }
        private async Task WaitForCameraConnectionAsync()
        {
            int timeout = 15000; // 15 seconds timeout
            int elapsed = 0;
            int checkInterval = 200;

            while (elapsed < timeout)
            {
                // ✅ Kiểm tra xem đã nhận frame đầu tiên chưa
                if (firstCameraFrameReceived)
                {
                    Debug.WriteLine("✅ Camera connected and first frame received!");
                    return;
                }

                await Task.Delay(checkInterval);
                elapsed += checkInterval;

                // ✅ Cập nhật progress
                if (elapsed % 2000 == 0) // Mỗi 2 giây
                {
                    string waitMsg = CultureManager.CurrentCultureCode.Contains("vi")
                        ? $"🎥 Đang chờ camera... ({elapsed / 1000}s)"
                        : $"🎥 Waiting for camera... ({elapsed / 1000}s)";
                    UpdateLoadingText(waitMsg);
                }
            }

            // ✅ Timeout - Hiển thị warning nhưng vẫn tiếp tục
            string timeoutMsg = CultureManager.CurrentCultureCode.Contains("vi")
                ? "⚠️ Không thể kết nối camera sau 15 giây.\nVui lòng kiểm tra lại."
                : "⚠️ Camera connection timeout after 15 seconds.\nPlease check your camera.";

            Debug.WriteLine("⚠️ Camera connection timeout!");

            // Hiển thị warning trong UI
            this.Invoke(new Action(() =>
            {
                CustomMessageBox.ShowWarning(timeoutMsg, "Camera Warning");
            }));
        }
    }
}
