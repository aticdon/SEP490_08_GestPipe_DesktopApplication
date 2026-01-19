namespace GestPipe.Backend.Models
{
    public class UserGestureRequestDto
    {
        public string UserId { get; set; }
        public string UserGestureConfigId { get; set; }
        public string GestureTypeId { get; set; }
        public string PoseLabel { get; set; }
        public Dictionary<string, string> Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
