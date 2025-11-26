namespace GestPipe.Backend.Models.DTOs
{
    public class DriveSyncProgress
    {
        public int TotalFiles { get; set; }
        public int SyncedFiles { get; set; }
        public bool IsCompleted { get; set; }
    }
}
