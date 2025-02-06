using System.Collections.Generic;

namespace Presentation_UI_.ViewModels
{
    public class AdminPageViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public List<ServiceViewModel> Services { get; set; } = new List<ServiceViewModel>();
        public List<ProjectLeaderViewModel> ProjectLeaders { get; set; } = new List<ProjectLeaderViewModel>();
    }
}
