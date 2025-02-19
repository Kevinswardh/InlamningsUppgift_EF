using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Presentation_UI_.ViewModels;
using Business.DTOs;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace Presentation_UI_.Controllers
{
    /// <summary>
    /// Handles the creation of new projects, including project information input, validation, and saving to the database.
    /// </summary>
    public class CreatePageController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<CreatePageController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IServiceService _service;
        private readonly IProjectLeaderService _leaderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePageController"/> with the required services and logger.
        /// </summary>
        /// <param name="projectService">Service for project-related operations.</param>
        /// <param name="logger">Logger for logging information and errors.</param>
        /// <param name="customerService">Service for customer operations.</param>
        /// <param name="service">Service for handling available services.</param>
        /// <param name="projectLeaderService">Service for project leader management.</param>
        public CreatePageController(IProjectService projectService, ILogger<CreatePageController> logger, ICustomerService customerService, IServiceService service, IProjectLeaderService projectLeaderService)
        {
            _projectService = projectService;
            _logger = logger;
            _customerService = customerService;
            _service = service;
            _leaderService = projectLeaderService;
        }


        // GET: /CreatePage/
        /// <summary>
        /// Loads the project creation page with necessary data such as the next project number, project leaders, services, and customers.
        /// </summary>
        /// <returns>The view populated with a <see cref="ProjectCreateViewModel"/> for project creation.</returns>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index()-metoden anropades.");

            var nextProjectNumber = await _projectService.GetNextProjectNumberAsync();
            _logger.LogInformation($"Nästa projektnummer: {nextProjectNumber}");

            var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
            var services = await _service.GetAllServicesAsync();
            var customers = await _customerService.GetAllCustomersAsync();
            var model = new ProjectCreateViewModel
            {
                ProjectNumber = nextProjectNumber,
                StartDate = DateTime.Today,
                ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    FirstName = pl.FirstName,
                    LastName = pl.LastName,
                    Email = pl.Email,
                    Phone = pl.Phone,
                    Department = pl.Department
                }).ToList(),



                Services = services.Select(s => new ServiceViewModel
                {
                    ServiceID = s.ServiceID,
                    ServiceName = s.ServiceName
                }).ToList(),
                Customers = customers.Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    CustomerName = c.CustomerName
                }).ToList(),
                Orders = new List<OrderViewModel>(),
                Summary = new SummaryViewModel()
            };


            _logger.LogInformation("ViewModel för Index skapad och returneras.");
            return View(model);
        }


        /// <summary>
        /// Saves a new project to the database after validating the provided project creation view model.
        /// </summary>
        /// <param name="model">The <see cref="ProjectCreateViewModel"/> containing project details.</param>
        /// <returns>
        /// Redirects to the home page on success, or reloads the project creation page with validation errors.
        /// </returns>
        /// <remarks>
        /// Validation checks include project leader selection, project description, start date, and status.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Save(ProjectCreateViewModel model)
        {
            _logger.LogInformation("Save()-metoden anropades.");

            // 🔍 Hämta rätt projektledare från databasen
            var projectLeaders = await _leaderService.GetAllProjectLeadersAsync();
            var selectedProjectLeader = projectLeaders.FirstOrDefault(pl => pl.ProjectLeaderID == model.ProjectLeaderID);

            if (selectedProjectLeader == null)
            {
                _logger.LogError($"Felaktigt ProjectLeaderID: {model.ProjectLeaderID}. Finns inte i databasen.");
                ModelState.AddModelError("ProjectLeaderID", "Vald projektledare är ogiltig.");
            }

            // 🔍 Validering av projektinfo
            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError("Description", "Benämning är obligatorisk.");

            if (model.StartDate == default)
                ModelState.AddModelError("StartDate", "Startdatum är obligatoriskt.");

            if (string.IsNullOrWhiteSpace(model.Status))
                ModelState.AddModelError("Status", "Status är obligatoriskt.");

            if (model.ProjectLeaderID == 0)
                ModelState.AddModelError("ProjectLeaderID", "Välj en giltig projektledare.");

            // Om ModelState är ogiltig, ladda om sidan med felmeddelanden
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState är ogiltig, returnerar vy med felmeddelanden.");

                // Ladda om dropdown-listor för vyn
                model.ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    FirstName = pl.FirstName,
                    LastName = pl.LastName
                }).ToList();

                // Ladda tjänster och kunder tillbaka till modellen
                var services = await _service.GetAllServicesAsync();
                var customers = await _customerService.GetAllCustomersAsync();

                model.Services = services.Select(s => new ServiceViewModel
                {
                    ServiceID = s.ServiceID,
                    ServiceName = s.ServiceName
                }).ToList();

                model.Customers = customers.Select(c => new CustomerViewModel
                {
                    CustomerID = c.CustomerID,
                    CustomerName = c.CustomerName
                }).ToList();

                return View("Index", model);
            }

            // ✅ Skapa DTO och spara i databasen
            var projectDto = new ProjectDTO
            {
                ProjectNumber = model.ProjectNumber,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ProjectLeaderID = model.ProjectLeaderID,

                // 🔹 Sätt FirstName & LastName för att skapa dynamiskt ProjectLeaderName
                ProjectLeaderFirstName = selectedProjectLeader.FirstName,
                ProjectLeaderLastName = selectedProjectLeader.LastName,

                Orders = model.Orders.Select(o => new OrderDTO
                {
                    CustomerID = o.CustomerID,
                    ServiceID = o.ServiceID,
                    ProjectID = model.ProjectID,
                    Hours = o.Hours,
                    Price = o.Price
                }).ToList(),

                Summary = new SummaryDTO
                {
                    TotalHours = (int)(model.Summary?.TotalHours ?? 0),
                    TotalPrice = model.Summary?.TotalPrice ?? 0,
                    Notes = model.Summary?.Notes
                }
            };

            await _projectService.CreateProjectAsync(projectDto);

            _logger.LogInformation("Projektet har sparats i databasen. Omdirigerar till Home.");
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// Cancels the project creation process and redirects to the home page.
        /// </summary>
        /// <returns>A redirection to the home page.</returns>
        [HttpGet]
        public IActionResult Cancel()
        {
            _logger.LogInformation("Cancel()-metoden anropades. Omdirigerar till Home.");
            return RedirectToAction("Index", "Home");
        }
    }
}
