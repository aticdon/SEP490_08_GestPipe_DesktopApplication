using GestPipePowerPonit.Views.Auth;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            AppSettings.SetLanguage("EN");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string currentUserId = Properties.Settings.Default.UserId; // Thay thế bằng ID người dùng thực tế
            //string currentUserId = "68fa3209582c17a482c5b11e";

            // Lấy ngôn ngữ của user trước khi tạo form
            string lang = "en-US"; // mặc định
            try
            {
                using (var apiClient = new ApiClient("https://localhost:7219"))
                {
                    // Gọi API lấy user (chặn đồng bộ vì không dùng async Main)
                    var userTask = apiClient.GetUserAsync(currentUserId);
                    userTask.Wait();
                    var user = userTask.Result;
                    if (user != null && !string.IsNullOrWhiteSpace(user.UiLanguage))
                    {
                        lang = user.UiLanguage;
                    }
                }
            }
            catch
            {
                // Nếu lỗi thì dùng mặc định
                lang = "en-US";
            }

            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(lang);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(lang);
            
            //HomeUser homeFomr = new HomeUser(currentUserId);
            //Application.Run(homeFomr);
            // Chạy form chính
            var loginForm = new LoginForm();
            Application.Run(loginForm);
        }
    }
}
