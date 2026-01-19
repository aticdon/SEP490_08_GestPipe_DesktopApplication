namespace GestPipe.Backend.Models.DTOs
{
    public class DriveUploadProgress
    {
        public int TotalFiles { get; set; }
        public int UploadedFiles { get; set; }
        public bool IsCompleted { get; set; }
    }
}
