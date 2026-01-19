using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestPipePowerPonit.Models.DTOs
{
    public class DriveSyncProgress
    {
        public int TotalFiles { get; set; }
        public int SyncedFiles { get; set; }
        public bool IsCompleted { get; set; }
    }
}
