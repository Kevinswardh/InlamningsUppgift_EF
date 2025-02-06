namespace Business.DTOs
{
    public class SummaryDTO
    {
        public int SummaryID { get; set; }
        public int ProjectID { get; set; }
        public int TotalHours { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }
}
