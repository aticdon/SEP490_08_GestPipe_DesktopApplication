namespace GestPipe.Backend.Models.DTOs
{
    public class ImportGestureFromCsvRequest
    {
        public string UserId { get; set; }
        public string CsvContent { get; set; }
    }
}
