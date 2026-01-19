namespace GestPipe.Backend.Models.DTOs
{
    public class LatestRequestBatchDto
    {
        public string UserId { get; set; }
        public List<string> GestureConfigIds { get; set; }
    }
}
