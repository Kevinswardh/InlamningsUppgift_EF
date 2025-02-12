using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation_UI_.ViewModels;
using Business.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation_UI_.Controllers
{

    public class EditPageController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<EditPageController> _logger;  // Lägg till loggern
        private readonly IServiceService _service;
        private readonly ICustomerService _customerService;

        // Konstruktor
        public EditPageController(IProjectService projectService, ILogger<EditPageController> logger, IServiceService service, ICustomerService customerService)
        {
            _projectService = projectService;
            _logger = logger;  // Tilldela loggern
            _service = service;
            _customerService = customerService;
        }


        // GET: EditPage/Index/5
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            _logger.LogInformation("Försöker hämta projekt med ID: {ProjectId}", id);  // Logga försök att hämta projektet

            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                _logger.LogWarning("Projekt med ID: {ProjectId} hittades inte.", id);  // Logga varning om projektet inte finns
                return NotFound();  // Om projektet inte finns, visa 404
            }

            // Skapa viewmodellen och logga information om projektet
            _logger.LogInformation("Projekt med ID: {ProjectId} hämtades och ska redigeras.", id);

            var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
            var services = await _service.GetAllServicesAsync();
            var customers = await _customerService.GetAllCustomersAsync();

            var viewModel = new ProjectCreateViewModel
            {
                ProjectID = project.ProjectID,
                ProjectNumber = project.ProjectNumber,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                ProjectLeaderID = project.ProjectLeaderID,
                ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    Name = pl.Name
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

            return View(viewModel);
        }



        // POST: EditPage/SaveEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(ProjectCreateViewModel model)
        {
            _logger.LogInformation("Försöker spara projekt med ID: {ProjectId}.", model.ProjectID);

            // Validering av Orders, om det inte finns några order ska en felmeddelande läggas till
            if (model.Orders == null || !model.Orders.Any())
            {
                ModelState.AddModelError("Orders", "Minst en beställning krävs.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Logga detaljer om modellen innan vi uppdaterar projektet
                    _logger.LogInformation("Uppdaterar projekt med ID: {ProjectId}. Orders count: {OrdersCount}", model.ProjectID, model.Orders?.Count ?? 0);

                    var projectDto = new ProjectDTO
                    {
                        ProjectID = model.ProjectID,
                        ProjectNumber = model.ProjectNumber,
                        Description = model.Description,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Status = model.Status,
                        ProjectLeaderID = model.ProjectLeaderID,
                        Orders = model.Orders.Select(o => new OrderDTO
                        {
                            CustomerID = o.CustomerID,
                            ServiceID = o.ServiceID,
                            ProjectID = model.ProjectID,
                            Hours = o.Hours,
                            Price = o.Price,
                            ServiceName = o.ServiceName // ✅ Ta ServiceName direkt från Orders
                        }).ToList(),


                        Summary = new SummaryDTO
                        {
                            TotalHours = model.Summary?.TotalHours ?? 0,
                            TotalPrice = model.Summary?.TotalPrice ?? 0,
                            Notes = model.Summary?.Notes
                        }
                    };

                    // Uppdatera projektet i databasen
                    await _projectService.UpdateProjectAsync(projectDto);
                    _logger.LogInformation("Projekt med ID: {ProjectId} uppdaterades framgångsrikt.", model.ProjectID);

                    // Om allt går bra, gör en redirect till Index
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    // Logga ett fel om uppdateringen misslyckas
                    _logger.LogError(ex, "Ett fel inträffade vid uppdatering av projekt med ID: {ProjectId}.", model.ProjectID);
                    ModelState.AddModelError(string.Empty, "Ett fel inträffade vid uppdateringen av projektet.");
                }
            }
            else
            {
                // Logga om modellvalideringen misslyckas
                _logger.LogWarning("Modellvalidering misslyckades för projekt med ID: {ProjectId}.", model.ProjectID);
            }

            // Om modellvalideringen misslyckas eller något gick fel, skicka tillbaka modellen med felmeddelanden till vyn
            return View("Index", model); // Skicka rätt modell (ProjectCreateViewModel) tillbaka till vyn
        }


    }
}
