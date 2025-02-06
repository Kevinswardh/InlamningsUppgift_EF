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

        // Lista av ProjectLeaderViewModel för dropdown i vyn
        public List<ProjectLeaderViewModel> ProjectLeaders { get; set; } = new List<ProjectLeaderViewModel>();

        // Andra navigeringsegenskaper
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
        public SummaryViewModel Summary { get; set; }
    }
}
