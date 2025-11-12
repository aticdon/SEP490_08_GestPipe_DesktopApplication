using System;
using System.IO;
using System.Text.Json;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public static class AppSettings
    {
        private static string _currentLanguage = "EN"; // Mặc định tiếng Anh
        private static string _configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GestPipePowerPonit",
            "AppConfig.json"
        );

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            set => _currentLanguage = value?.ToUpper() ?? "EN";
        }

        /// <summary>
        /// Tải ngôn ngữ từ config file (được gọi khi user login thành công)
        /// </summary>
        public static void LoadLanguageSettings()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _currentLanguage = "EN";
                    return;
                }

                string json = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<AppConfig>(json);
                _currentLanguage = config?.Language ?? "EN";
            }
            catch
            {
                _currentLanguage = "EN";
            }
        }

        /// <summary>
        /// Lưu ngôn ngữ vào config file (được gọi khi user đổi ngôn ngữ ở Main Form)
        /// </summary>
        public static void SaveLanguageSettings(string language)
        {
            try
            {
                _currentLanguage = language?.ToUpper() ?? "EN";

                var config = new AppConfig { Language = _currentLanguage };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                string directory = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lưu language: {ex.Message}");
            }
        }
        public static string GetText(string resourceKey)
        {
            try
            {
                string fullKey = $"{resourceKey}_{CurrentLanguage}";
                string resourceNamespace = "GestPipePowerPonit.Languages";

                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceTypes = assembly.GetTypes()
                    .Where(t => t.Namespace == resourceNamespace && t.Name.EndsWith("Strings"))
                    .ToList();

                foreach (var type in resourceTypes)
                {
                    try
                    {
                        var resourceManager = type
                            .GetProperty("ResourceManager",
                                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                            ?.GetValue(null) as System.Resources.ResourceManager;

                        if (resourceManager != null)
                        {
                            string value = resourceManager.GetString(fullKey, CultureInfo.CurrentUICulture);
                            if (!string.IsNullOrEmpty(value))
                            {
                                System.Diagnostics.Debug.WriteLine($"[AppSettings] Found: {fullKey} in {type.Name}");
                                return value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AppSettings] Error checking {type.Name}: {ex.Message}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[AppSettings] NOT FOUND: {fullKey} in any resource file");
                return $"[{fullKey} not found]";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppSettings] GetText Error: {resourceKey} - {ex.Message}");
                return $"[Error: {ex.Message}]";
            }
        }

        /// <summary>
        /// Đổi ngôn ngữ (chỉ gọi từ Main Form hoặc Settings)
        /// </summary>
        public static void SetLanguage(string language)
        {
            string lang = language == "VI" ? "VI" : "EN";
            SaveLanguageSettings(lang);
            CurrentLanguage = lang;
            var culture = lang == "VI"
        ? new System.Globalization.CultureInfo("vi-VN")
        : new System.Globalization.CultureInfo("en-US");

            System.Globalization.CultureInfo.CurrentUICulture = culture;
            System.Globalization.CultureInfo.CurrentCulture = culture;
        }

        public class AppConfig
        {
            public string Language { get; set; } = "EN";
            public string Username { get; set; }
            public DateTime LastLogin { get; set; }
        }

        public static void ExitAll()
        {
            Application.Exit();
        }
    }
}