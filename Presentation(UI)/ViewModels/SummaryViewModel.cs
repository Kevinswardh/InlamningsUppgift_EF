namespace Presentation_UI_.ViewModels
{
    public class SummaryViewModel
    {
        public int SummaryID { get; set; }
        public int ProjectID { get; set; }
        public int TotalHours { get; set; }
        public decimal TotalPrice { get; set; }
        public string Notes { get; set; }
    }
}
