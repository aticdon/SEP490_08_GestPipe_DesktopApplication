namespace GestPipe.Backend.Models
{
    public class GestureStats
    {
        public int TotalGestures { get; set; }
        public int ActiveGestures { get; set; }
        public int InactiveGestures { get; set; }
        public double AverageAccuracy { get; set; }
    }
}
