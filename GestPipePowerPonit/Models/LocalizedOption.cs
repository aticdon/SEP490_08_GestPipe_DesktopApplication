using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models
{
    public class LocalizedOption
    {
        public string Key { get; }
        public string TextEn { get; }
        public string TextVi { get; }

        public LocalizedOption(string key, string textEn, string textVi)
        {
            Key = key;
            TextEn = textEn;
            TextVi = textVi;
        }
    }
}
