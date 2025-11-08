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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string currentUserId = "68fa3209582c17a482c5b11e"; // Thay thế bằng ID người dùng thực tế

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
                    if (user != null && !string.IsNullOrWhiteSpace(user.Language))
                    {
                        lang = user.Language;
                    }
                }
            }
            catch
            {
                // Nếu lỗi thì dùng mặc định
                lang = "en-US";
            }

            // Set culture trước khi khởi tạo form
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(lang);
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(lang);

            // Tạo form sau khi đã set culture
            Application.Run(new HomeUser(currentUserId));
            //Application.Run(new TrainingGesture());
        }
    }
}