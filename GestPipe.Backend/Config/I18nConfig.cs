using System.Collections.Generic;

namespace GestPipe.Backend.Config
{
    // Bind phần cấu hình "I18n" trong appsettings.json
    public class I18nConfig
    {
        // ví dụ: ["en-US", "vi-VN"]
        public List<string> SupportedCultures { get; set; } = new List<string> { "en-US" };

        // ví dụ: "en-US"
        public string DefaultCulture { get; set; } = "en-US";
    }
}