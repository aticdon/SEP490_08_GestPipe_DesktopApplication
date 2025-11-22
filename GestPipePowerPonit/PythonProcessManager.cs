using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GestPipePowerPonit
{
    public static class PythonProcessManager
    {
        public enum ProcessType
        {
            Custom,
            Training,
            Presentation
        }

        private static readonly Dictionary<string, Process> _processes = new Dictionary<string, Process>();
        private static readonly object _lock = new object();

        // ✅ Path configurations
        private static readonly string PythonExePath = @"C:\Users\Admin\AppData\Local\Programs\Python\Python311\python.exe";

        public static async Task<bool> EnsureProcessReadyAsync(ProcessType type, string userId)
        {
            string key = $"{type}_{userId}";

            lock (_lock)
            {
                if (_processes.ContainsKey(key) && !_processes[key].HasExited)
                {
                    Debug.WriteLine($"[PythonManager] Process {key} đã sẵn sàng");
                    return true;
                }
            }

            Debug.WriteLine($"[PythonManager] Khởi tạo process mới cho {key}");
            return await StartProcessAsync(type, userId);
        }

        private static async Task<bool> StartProcessAsync(ProcessType type, string userId)
        {
            try
            {
                string userFolder = $"user_{userId}";
                string scriptFile = GetScriptPath(type, userFolder);

                if (!File.Exists(scriptFile))
                {
                    Debug.WriteLine($"[PythonManager] Script không tồn tại: {scriptFile}");
                    return false;
                }

                var process = CreatePythonProcess(PythonExePath, scriptFile, type, userId);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                string key = $"{type}_{userId}";
                lock (_lock)
                {
                    if (_processes.ContainsKey(key))
                        _processes[key] = process;
                    else
                        _processes.Add(key, process);
                }

                // Wait a bit for process to initialize
                await Task.Delay(2000);

                Debug.WriteLine($"[PythonManager] Thành công khởi tạo {type} cho user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PythonManager] Lỗi khởi tạo {type}: {ex.Message}");
                return false;
            }
        }

        private static string GetScriptPath(ProcessType type, string userFolder)
        {
            string baseDir = @"D:\Semester9\codepython\hybrid_realtime_pipeline\code";

            switch (type)
            {
                case ProcessType.Custom:
                    return Path.Combine(baseDir, "collect_data_update.py");
                case ProcessType.Training:
                    return Path.Combine(baseDir, userFolder, "training_session_ml.py");
                case ProcessType.Presentation:
                    return Path.Combine(baseDir, userFolder, "test_gesture_recognition.py");
                default:
                    throw new ArgumentException($"Unsupported process type: {type}");
            }
        }

        private static Process CreatePythonProcess(string pythonExePath, string scriptFile, ProcessType type, string userId)
        {
            var process = new Process();
            process.StartInfo.FileName = pythonExePath;

            // ✅ Arguments với port riêng biệt và wait mode
            switch (type)
            {
                case ProcessType.Custom:
                    process.StartInfo.Arguments = $"\"{scriptFile}\" --wait-for-connection --socket-port 5300 --camera-port 5301 --status-port 5302";
                    break;
                case ProcessType.Training:
                    process.StartInfo.Arguments = $"\"{scriptFile}\" --wait-for-connection --pose-port 7100 --camera-port 7101 --status-port 7102";
                    break;
                case ProcessType.Presentation:
                    process.StartInfo.Arguments = $"\"{scriptFile}\" --wait-for-connection --headless --camera-port 6200 --command-port 6201";
                    break;
            }

            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptFile);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            // ✅ SỬA: EnableRaisingEvents là property của Process
            process.EnableRaisingEvents = true;

            // Set environment variables
            SetEnvironmentVariables(process, type, userId, scriptFile);

            // Event handlers
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine($"[{type} PYTHON OUT] {e.Data}");
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.WriteLine($"[{type} PYTHON ERR] {e.Data}");
            };

            // Cleanup khi process kết thúc
            process.Exited += (s, e) =>
            {
                Debug.WriteLine($"[PythonManager] Process {type}_{userId} đã kết thúc");
                lock (_lock)
                {
                    string key = $"{type}_{userId}";
                    if (_processes.ContainsKey(key))
                        _processes.Remove(key);
                }
            };

            return process;
        }

        private static void SetEnvironmentVariables(Process process, ProcessType type, string userId, string scriptFile)
        {
            // Clear and copy current environment
            process.StartInfo.EnvironmentVariables.Clear();
            foreach (System.Collections.DictionaryEntry env in Environment.GetEnvironmentVariables())
            {
                process.StartInfo.EnvironmentVariables.Add(env.Key.ToString(), env.Value.ToString());
            }

            // Common environment variables
            process.StartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";
            process.StartInfo.EnvironmentVariables["PYTHONUTF8"] = "1";
            process.StartInfo.EnvironmentVariables["WINFORM_USER_ID"] = userId;
            process.StartInfo.EnvironmentVariables["PYTHONPATH"] = Path.GetDirectoryName(scriptFile);
            process.StartInfo.EnvironmentVariables["WINFORM_SCRIPT_DIR"] = Path.GetDirectoryName(scriptFile);
        }

        public static void CleanupAll()
        {
            Debug.WriteLine("[PythonManager] Cleaning up all Python processes...");

            lock (_lock)
            {
                foreach (var kvp in _processes)
                {
                    try
                    {
                        if (!kvp.Value.HasExited)
                        {
                            Debug.WriteLine($"[PythonManager] Terminating {kvp.Key}");
                            kvp.Value.Kill();
                            kvp.Value.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[PythonManager] Error cleaning up {kvp.Key}: {ex.Message}");
                    }
                }
                _processes.Clear();
            }

            Debug.WriteLine("[PythonManager] Cleanup completed");
        }

        public static bool IsProcessRunning(ProcessType type, string userId)
        {
            string key = $"{type}_{userId}";
            lock (_lock)
            {
                return _processes.ContainsKey(key) && !_processes[key].HasExited;
            }
        }
    }
}