using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models.DTOs
{
    public class DriveUploadProgress
    {
        public int TotalFiles { get; set; }
        public int UploadedFiles { get; set; }
        public bool IsCompleted { get; set; }
    }
}
