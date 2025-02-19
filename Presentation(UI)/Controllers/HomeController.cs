using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation_UI_.Models;
using Presentation_UI_.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{
    /// <summary>
    /// Handles the main application pages, including the project overview, privacy policy, and error handling.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectService _projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> with the required services and logger.
        /// </summary>
        /// <param name="logger">Logger for logging application information and errors.</param>
        /// <param name="projectService">Service for handling project-related operations.</param>
        public HomeController(ILogger<HomeController> logger, IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }


        /// <summary>
        /// Loads the home page with a list of all projects, including their details, summaries, and orders.
        /// </summary>
        /// <returns>
        /// The view populated with a list of <see cref="ProjectViewModel"/> instances representing all projects.
        /// </returns>
        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();

            var model = projects.Select(p => new ProjectViewModel
            {
                ProjectID = p.ProjectID,
                ProjectNumber = p.ProjectNumber,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                ProjectLeaderID = p.ProjectLeaderID,
                ProjectLeaderFirstName = p.ProjectLeaderFirstName, // ✅ Tilldela FirstName separat
                ProjectLeaderLastName = p.ProjectLeaderLastName,   // ✅ Tilldela LastName separat

                Summary = p.Summary != null ? new SummaryViewModel
                {
                    SummaryID = p.Summary.SummaryID,
                    ProjectID = p.Summary.ProjectID,
                    TotalHours = p.Summary.TotalHours,
                    TotalPrice = p.Summary.TotalPrice,
                    Notes = p.Summary.Notes
                } : null,
                Orders = p.Orders?.Select(o => new OrderViewModel
                {
                    CustomerID = o.CustomerID,
                    ServiceID = o.ServiceID,
                    Hours = o.Hours,
                    Price = o.Price
                }).ToList() ?? new List<OrderViewModel>()
            }).ToList();

            return View(model);
        }



        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        /// <returns>The privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }


        /// <summary>
        /// Displays the error page with error details and request ID information.
        /// </summary>
        /// <returns>The error view with an <see cref="ErrorViewModel"/> containing the request ID.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
