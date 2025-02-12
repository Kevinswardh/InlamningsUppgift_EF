namespace Presentation_UI_.ViewModels
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string ProjectNumber { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int ProjectLeaderID { get; set; }
        public string ProjectLeaderName { get; set; } // Lägg till detta


        // Andra navigeringsegenskaper
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public SummaryViewModel? Summary { get; set; }
    }
}
