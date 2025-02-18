using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Presentation_UI_.ViewModels;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{
    public class ProjectPageController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectPageController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            var viewModel = new ProjectViewModel
            {
                ProjectID = project.ProjectID,
                ProjectNumber = project.ProjectNumber,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                ProjectLeaderID = project.ProjectLeaderID,
                ProjectLeaderFirstName = project.ProjectLeaderFirstName, // ✅ Ny egenskap
                ProjectLeaderLastName = project.ProjectLeaderLastName,   // ✅ Ny egenskap

                Orders = project.Orders.Select(o => new OrderViewModel
                {
                    CustomerID = o.CustomerID,
                    CustomerName = o.CustomerName,
                    ServiceID = o.ServiceID,
                    ServiceName = o.ServiceName,
                    Hours = o.Hours,
                    Price = o.Price,
                    ProjectID = o.ProjectID
                }).ToList(),

                Summary = project.Summary != null ? new SummaryViewModel
                {
                    TotalHours = project.Summary.TotalHours,
                    TotalPrice = project.Summary.TotalPrice,
                    Notes = project.Summary.Notes ?? "Inga anteckningar"
                } : new SummaryViewModel()
            };



            return View(viewModel);
        }
    }
}
