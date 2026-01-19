using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit
{
    public static class CultureManager
    {
        private static string _currentCultureCode = "en-US";
        public static string CurrentCultureCode
        {
            get => _currentCultureCode;
            set
            {
                if (_currentCultureCode != value)
                {
                    _currentCultureCode = value;
                    CultureChanged?.Invoke(null, EventArgs.Empty); 
                }
            }
        }
        public static event EventHandler CultureChanged;
    }
}
