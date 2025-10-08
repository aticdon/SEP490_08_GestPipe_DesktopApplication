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
            server = new TcpListener(IPAddress.Loopback, port);
            server.Start();
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
                catch { }
            }
            server.Stop();
        });
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    public void Stop()
    {
        isRunning = false;
        try
        {
            server?.Stop();
        }
        catch { }
    }
}