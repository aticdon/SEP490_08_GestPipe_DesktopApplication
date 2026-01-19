using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class Topic
    {
        public string Id { get; set; }
        public Dictionary<string, string> Title { get; set; }
        public string CategoryId { get; set; }
        public string DisplayTitle
        {
            get
            {
                var lang = GestPipePowerPonit.CultureManager.CurrentCultureCode ?? "en-US";
                // lấy tiền tố ngắn nếu có ngôn ngữ theo kiểu "vi" hoặc "en"
                var langShort = lang.Split('-')[0];

                if (Title != null)
                {
                    if (Title.ContainsKey(lang))    // "vi-VN"
                        return Title[lang];
                    if (Title.ContainsKey(langShort))   // "vi"
                        return Title[langShort];
                    if (Title.ContainsKey("vi"))
                        return Title["vi"];
                    if (Title.ContainsKey("en"))
                        return Title["en"];
                }
                return Title?.Values.FirstOrDefault() ?? "";
            }
        }
    }
}
