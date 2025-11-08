namespace GestPipe.Backend.Models
{
    public class DefaultGestureDto
    {
        public string Id { get; set; }
        public string Name { get; set; } // code của GestureType
        public string Type { get; set; }
        public double Accuracy { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
