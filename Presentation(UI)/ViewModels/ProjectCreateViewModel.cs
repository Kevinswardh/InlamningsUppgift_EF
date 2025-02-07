namespace Presentation_UI_.ViewModels
{
    public class ProjectCreateViewModel
    {
        // Projektinformation
        public int ProjectID { get; set; }
        public string ProjectNumber { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int ProjectLeaderID { get; set; }

        // DropDown-listor för Projektledare, Tjänster och Kunder
        public List<ProjectLeaderViewModel> ProjectLeaders { get; set; } = new List<ProjectLeaderViewModel>();
        public List<ServiceViewModel> Services { get; set; } = new List<ServiceViewModel>();
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();

        // Beställningar
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();

        // Sammanställning
        public SummaryViewModel Summary { get; set; } = new SummaryViewModel();
    }
}
