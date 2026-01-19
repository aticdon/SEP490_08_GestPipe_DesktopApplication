using Newtonsoft.Json;

namespace GestPipe.GestPipePowerPoint.Models.DTOs
{
    public class DriveSyncResult
    {
        [JsonProperty("totalFiles")]
        public int TotalFiles { get; set; }

        [JsonProperty("syncedFiles")]
        public int SyncedFiles { get; set; }
    }
}
