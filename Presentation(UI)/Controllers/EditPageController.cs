using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation_UI_.ViewModels;
using Business.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{

    /// <summary>
    /// Handles editing of existing projects, including loading project data, validating input, and saving updates to the database.
    /// </summary>
    public class EditPageController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<EditPageController> _logger;  // Lägg till loggern
        private readonly IServiceService _service;
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPageController"/> with the required services and logger.
        /// </summary>
        /// <param name="projectService">Service for project-related operations.</param>
        /// <param name="logger">Logger for logging information and errors.</param>
        /// <param name="service">Service for managing available services.</param>
        /// <param name="customerService">Service for customer-related operations.</param>
        public EditPageController(IProjectService projectService, ILogger<EditPageController> logger, IServiceService service, ICustomerService customerService)
        {
            _projectService = projectService;
            _logger = logger;  // Tilldela loggern
            _service = service;
            _customerService = customerService;
        }



        /// <summary>
        /// Loads the edit page for a specific project by ID, populating the view with project details, project leaders, services, and customers.
        /// </summary>
        /// <param name="id">The unique identifier of the project to edit.</param>
        /// <returns>
        /// The view populated with a <see cref="ProjectCreateViewModel"/> if the project is found; otherwise, a NotFound result.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            _logger.LogInformation("Försöker hämta projekt med ID: {ProjectId}", id);

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                _logger.LogWarning("Projekt med ID: {ProjectId} hittades inte.", id);
                return NotFound();
            }

            _logger.LogInformation("Projekt med ID: {ProjectId} hämtades och ska redigeras.", id);

            // 🔹 Hämta alla nödvändiga data
            var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
            var services = await _service.GetAllServicesAsync();
            var customers = await _customerService.GetAllCustomersAsync();

            // 🔹 Placera vald projektledare först i listan
            var sortedProjectLeaders = projectLeaders
                .OrderByDescending(pl => pl.ProjectLeaderID == project.ProjectLeaderID) // Vald ID kommer först
                .ThenBy(pl => pl.FirstName) // Resten sorteras på förnamn
                .Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    FirstName = pl.FirstName,
                    LastName = pl.LastName,
                    IsDeleted = pl.IsDeleted,
                }).ToList();

            // 🔹 Skapa och returnera ViewModel
            var viewModel = new ProjectCreateViewModel
            {
                ProjectID = project.ProjectID,
                ProjectNumber = project.ProjectNumber,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                ProjectLeaderID = project.ProjectLeaderID,  // ✅ Se till att den rätta projektledaren är vald
                ProjectLeaders = sortedProjectLeaders,      // ✅ Använd den sorterade listan

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

                Orders = project.Orders.Select(o => new OrderViewModel
                {
                    CustomerID = o.CustomerID,
                    ServiceID = o.ServiceID,
                    Hours = o.Hours,
                    Price = o.Price,
                    ServiceName = services.FirstOrDefault(s => s.ServiceID == o.ServiceID)?.ServiceName ?? "Ej tilldelad",
                    CustomerName = customers.FirstOrDefault(c => c.CustomerID == o.CustomerID)?.CustomerName ?? "Ej tilldelad"
                }).ToList(),

                Summary = new SummaryViewModel
                {
                    TotalHours = project.Summary?.TotalHours ?? 0,
                    TotalPrice = project.Summary?.TotalPrice ?? 0,
                    Notes = project.Summary?.Notes ?? "Inga anteckningar"
                }
            };

            _logger.LogInformation("Antal projektledare i ViewModel: {Count}", viewModel.ProjectLeaders.Count);
            return View(viewModel);

        }

        /// <summary>
        /// Saves the edited project details to the database after validating the provided project creation view model.
        /// </summary>
        /// <param name="model">The <see cref="ProjectCreateViewModel"/> containing the updated project information.</param>
        /// <returns>
        /// Redirects to the home page on success, or reloads the edit page with validation errors if the model is invalid.
        /// </returns>
        /// <remarks>
        /// This method checks for valid project leader selection, required fields, and ensures at least one order is present.
        /// It also handles updating existing orders and adding new ones.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(ProjectCreateViewModel model)
        {
            _logger.LogInformation("Försöker spara redigerat projekt med ID: {ProjectId}.", model.ProjectID);

            // Kontrollera om vald projektledare finns i databasen
            var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
            var selectedProjectLeader = projectLeaders.FirstOrDefault(pl => pl.ProjectLeaderID == model.ProjectLeaderID);

            if (selectedProjectLeader == null)
            {
                _logger.LogError($"Felaktigt ProjectLeaderID: {model.ProjectLeaderID}. Finns inte i databasen.");
                ModelState.AddModelError("ProjectLeaderID", "Vald projektledare är ogiltig.");
            }

            // Validering av projektinfo
            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError("Description", "Benämning är obligatorisk.");

            if (model.StartDate == default)
                ModelState.AddModelError("StartDate", "Startdatum är obligatoriskt.");

            if (string.IsNullOrWhiteSpace(model.Status))
                ModelState.AddModelError("Status", "Status är obligatoriskt.");

            if (model.ProjectLeaderID == 0)
                ModelState.AddModelError("ProjectLeaderID", "Välj en giltig projektledare.");

            // Validering av beställningar
            if (model.Orders == null || !model.Orders.Any())
            {
                ModelState.AddModelError("Orders", "Minst en beställning krävs.");
            }

            // Om ModelState är ogiltigt, ladda om sidan med felmeddelanden
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modellvalidering misslyckades för projekt med ID: {ProjectId}.", model.ProjectID);

                // Ladda om dropdown-listor för vyn
                model.ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    FirstName = pl.FirstName,
                    LastName = pl.LastName
                }).ToList();

                return View("Index", model);
            }

            // Skapa DTO och uppdatera projektet i databasen
            var projectDto = new ProjectDTO
            {
                ProjectID = model.ProjectID,
                ProjectNumber = model.ProjectNumber,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ProjectLeaderID = model.ProjectLeaderID,

                // Sätt FirstName & LastName för att skapa dynamiskt ProjectLeaderName
                ProjectLeaderFirstName = selectedProjectLeader.FirstName,
                ProjectLeaderLastName = selectedProjectLeader.LastName,

                Orders = new List<OrderDTO>()  // Starta med en tom lista för orders
            };

            // Hämta det existerande projektet, inklusive beställningar
            var existingProject = await _projectService.GetProjectByIdAsync(model.ProjectID);

            if (existingProject == null)
            {
                _logger.LogError("Projekt med ID: {ProjectId} kunde inte hittas.", model.ProjectID);
                return NotFound();
            }

            // Hämta alla nuvarande beställningar från det existerande projektet
            var existingOrders = existingProject.Orders;

            foreach (var order in model.Orders)
            {
                // Försök att hitta den existerande beställningen baserat på nycklarna
                var existingOrder = existingOrders.FirstOrDefault(o => o.ProjectID == order.ProjectID
                                                                       && o.CustomerID == order.CustomerID
                                                                       && o.ServiceID == order.ServiceID);

                if (existingOrder != null)
                {
                    // Uppdatera den redan spårade ordern
                    existingOrder.Hours = order.Hours;
                    existingOrder.Price = order.Price;
                    existingOrder.ServiceName = order.ServiceName;
                    existingOrder.CustomerName = order.CustomerName;

                    // Lägg till den uppdaterade beställningen till DTO:n
                    projectDto.Orders.Add(existingOrder);  // EF Core hanterar den redan spårade entiteten
                }
                else
                {
                    // Lägg till nya beställningar som inte finns i databasen
                    var newOrder = new OrderDTO
                    {
                        ProjectID = order.ProjectID,
                        CustomerID = order.CustomerID,
                        ServiceID = order.ServiceID,
                        Hours = order.Hours,
                        Price = order.Price,
                        ServiceName = order.ServiceName,
                        CustomerName = order.CustomerName
                    };
                    projectDto.Orders.Add(newOrder);  // Lägg till den nya beställningen till DTO:n
                }
            }

            // ✅ Uppdatera projektet
            await _projectService.UpdateProjectAsync(projectDto);

            _logger.LogInformation("Projekt med ID: {ProjectId} uppdaterades framgångsrikt.", model.ProjectID);

            return RedirectToAction("Index", "Home");
        }





    }
}
