using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class Category
    {
        public string Id { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public string DisplayName
        {
            get
            {
                var lang = GestPipePowerPonit.CultureManager.CurrentCultureCode ?? "en-US";
                // lấy tiền tố ngắn nếu có ngôn ngữ theo kiểu "vi" hoặc "en"
                var langShort = lang.Split('-')[0];

                if (Name != null)
                {
                    if (Name.ContainsKey(lang))    // "vi-VN"
                        return Name[lang];
                    if (Name.ContainsKey(langShort))   // "vi"
                        return Name[langShort];
                    if (Name.ContainsKey("vi"))
                        return Name["vi"];
                    if (Name.ContainsKey("en"))
                        return Name["en"];
                }
                return Name?.Values.FirstOrDefault() ?? "";
            }
        }
    }
}
