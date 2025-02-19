using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Presentation_UI_.ViewModels;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{
    /// <summary>
    /// Handles the display of detailed project information, including orders, summaries, and project leader details.
    /// </summary>
    public class ProjectPageController : Controller
    {
        private readonly IProjectService _projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectPageController"/> with the required project service.
        /// </summary>
        /// <param name="projectService">Service for handling project-related operations.</param>
        public ProjectPageController(IProjectService projectService)
        {
            _projectService = projectService;
        }


        /// <summary>
        /// Loads the project details page for a specified project ID, including related orders and summary information.
        /// </summary>
        /// <param name="id">The unique identifier of the project to display.</param>
        /// <returns>
        /// The view populated with a <see cref="ProjectViewModel"/> containing project details, or a NotFound result if the project does not exist.
        /// </returns>
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
