using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GestPipe.Launcher
{
    class Program
    {
        private const string BACKEND_EXE = "Backend\\GestPipe.Backend.exe";
        private const string CLIENT_EXE = "Client\\GestPipePowerPoint.exe";
        private const string BACKEND_URL = "http://localhost:5083";
        private const int BACKEND_PORT = 5083;

        private static Process? backendProcess;
        private static Process? clientProcess;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "GestPipe Application Launcher";
            PrintBanner();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnCancelKeyPress;

            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[1/3] Checking Backend API...");
                Console.ResetColor();

                if (IsPortInUse(BACKEND_PORT))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"     ✓ Backend API already running on port {BACKEND_PORT}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[2/3] Starting Backend API...");
                    Console.ResetColor();

                    if (!StartBackend())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("     ✗ Failed to start Backend API");
                        Console.ResetColor();
                        return;
                    }
                    Console.Write("     Waiting for Backend to start");
                    if (await WaitForBackendAsync(BACKEND_URL, 30))
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("     ✓ Backend API started successfully!");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("     ⚠ Backend may not be fully ready, but continuing...");
                        Console.ResetColor();
                    }
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[3/3] Starting WinForms Client...");
                Console.ResetColor();

                if (!StartClient())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("     ✗ Failed to start WinForms Client");
                    Console.ResetColor();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("     ✓ Client started successfully!");
                Console.ResetColor();

                PrintSuccessMessage();

                Console.WriteLine("\nWaiting for client to close...");
                Console.WriteLine("(Press Ctrl+C to force shutdown both applications)");
                clientProcess?.WaitForExit();

                Console.WriteLine("\nClient closed by user.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ ERROR: {ex.Message}");
                Console.WriteLine($"Details: {ex.StackTrace}");
                Console.ResetColor();
            }
            finally
            {
                Cleanup();
            }
        }

        private static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═════════════════════════════════════════════╗");
            Console.WriteLine("║           GestPipe Launcher                ║");
            Console.WriteLine("╚═════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void PrintSuccessMessage()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Application Started Successfully!");
            Console.ResetColor();
            Console.WriteLine($"Backend API:  {BACKEND_URL}");
            Console.WriteLine($"Swagger UI:   {BACKEND_URL}/swagger");
            Console.WriteLine("Client:       Running");
        }

        private static bool StartBackend()
        {
            try
            {
                var backendPath = Path.GetFullPath(BACKEND_EXE);
                var backendDir = Path.GetDirectoryName(backendPath);

                if (!File.Exists(backendPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"     ✗ Backend executable not found: {backendPath}");
                    Console.ResetColor();
                    return false;
                }

                backendProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = backendPath,
                        WorkingDirectory = backendDir,
                        UseShellExecute = false,
                        CreateNoWindow = false,
                        WindowStyle = ProcessWindowStyle.Minimized,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false
                    }
                };

                backendProcess.Start();
                Thread.Sleep(2000);
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"     ✗ Error starting backend: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        private static bool StartClient()
        {
            try
            {
                var clientPath = Path.GetFullPath(CLIENT_EXE);
                var clientDir = Path.GetDirectoryName(clientPath);

                if (!File.Exists(clientPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"     ✗ Client executable not found: {clientPath}");
                    Console.ResetColor();
                    return false;
                }

                clientProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = clientPath,
                        WorkingDirectory = clientDir,
                        UseShellExecute = true
                    }
                };

                clientProcess.Start();
                Thread.Sleep(1000);
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"     ✗ Error starting client: {ex.Message}");
                Console.ResetColor();
                return false;
            }
        }

        private static bool IsPortInUse(int port)
        {
            try
            {
                using var client = new TcpClient();
                client.Connect("127.0.0.1", port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> WaitForBackendAsync(string url, int timeoutSeconds)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            var endTime = DateTime.Now.AddSeconds(timeoutSeconds);

            while (DateTime.Now < endTime)
            {
                try
                {
                    var response = await client.GetAsync($"{url}/health");
                    if (response.IsSuccessStatusCode)
                        return true;
                }
                catch { }
                Console.Write(".");
                await Task.Delay(1000);
            }
            return false;
        }

        private static void Cleanup()
        {
            Console.WriteLine("Shutting down applications...");
            try
            {
                if (clientProcess != null && !clientProcess.HasExited)
                {
                    clientProcess.Kill(entireProcessTree: true);
                    clientProcess.WaitForExit(3000);
                    Console.WriteLine("Stopped client.");
                }

                if (backendProcess != null && !backendProcess.HasExited)
                {
                    backendProcess.Kill(entireProcessTree: true);
                    backendProcess.WaitForExit(3000);
                    Console.WriteLine("Stopped backend.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n⚠ Cleanup warning: {ex.Message}");
            }
        }

        private static void OnProcessExit(object? sender, EventArgs e)
        {
            Cleanup();
        }

        private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Cleanup();
            Environment.Exit(0);
        }
    }
}