namespace GestPipe.Backend.Models
{
    public class DefaultGestureDto
    {
        public string Id { get; set; }
        public Dictionary<string, string> Name { get; set; } // Chuẩn đa ngôn ngữ
        public Dictionary<string, string> Type { get; set; }
        public double Accuracy { get; set; }
        public Dictionary<string, string> Status { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
