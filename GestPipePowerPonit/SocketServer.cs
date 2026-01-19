using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public class SocketServer
{
    private TcpListener server;
    private Thread serverThread;
    private bool isRunning = false;
    private int port;
    private Form uiForm;
    private Action<string> onCommandReceived;

    public SocketServer(int port = 5006, Form uiForm = null, Action<string> onCommandReceived = null)
    {
        this.port = port;
        this.uiForm = uiForm;
        this.onCommandReceived = onCommandReceived;
    }

    public void Start()
    {
        if (isRunning) return;

        isRunning = true;
        serverThread = new Thread(() =>
        {
            try
            {
                server = new TcpListener(IPAddress.Loopback, port);
                server.Start();
                System.Diagnostics.Debug.WriteLine($"[SocketServer] Started on port {port}");

                while (isRunning)
                {
                    try
                    {
                        if (!server.Pending())
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        TcpClient client = server.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[256];
                        int bytes = stream.Read(buffer, 0, buffer.Length);
                        string command = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();

                        // Đảm bảo callback chạy trên UI thread
                        if (uiForm != null && uiForm.InvokeRequired)
                        {
                            uiForm.Invoke(new Action(() =>
                            {
                                onCommandReceived?.Invoke(command);
                            }));
                        }
                        else
                        {
                            onCommandReceived?.Invoke(command);
                        }

                        // Gửi phản hồi về client (nếu cần)
                        byte[] response = Encoding.UTF8.GetBytes("OK");
                        stream.Write(response, 0, response.Length);

                        stream.Close();
                        client.Close();
                    }
                    catch (SocketException ex)
                    {
                        if (isRunning) // Chỉ log nếu đang chạy
                        {
                            System.Diagnostics.Debug.WriteLine($"[SocketServer] Socket error: {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[SocketServer] Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SocketServer] Thread error: {ex.Message}");
            }
            finally
            {
                try
                {
                    server?.Stop();
                    System.Diagnostics.Debug.WriteLine($"[SocketServer] Stopped");
                }
                catch { }
            }
        });

        serverThread.IsBackground = true;
        serverThread.Start();
    }

    // ✅ SỬA PHƯƠNG THỨC STOP
    public void Stop()
    {
        if (!isRunning) return;

        System.Diagnostics.Debug.WriteLine("[SocketServer] Stopping...");

        isRunning = false;

        try
        {
            // ✅ Đóng server trước
            server?.Stop();
            System.Diagnostics.Debug.WriteLine("[SocketServer] Server stopped");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SocketServer] Error stopping server: {ex.Message}");
        }

        try
        {
            // ✅ Chờ thread thoát
            if (serverThread != null && serverThread.IsAlive)
            {
                if (!serverThread.Join(2000)) // Chờ 2 giây
                {
                    System.Diagnostics.Debug.WriteLine("[SocketServer] Thread did not stop, aborting...");
                    serverThread.Abort();
                }
            }
            System.Diagnostics.Debug.WriteLine("[SocketServer] Thread stopped");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SocketServer] Error stopping thread: {ex.Message}");
        }

        // ✅ Nullify references
        server = null;
        serverThread = null;

        System.Diagnostics.Debug.WriteLine("[SocketServer] Cleanup complete");
    }
}