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
    public class CreatePageController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<CreatePageController> _logger;

        public CreatePageController(IProjectService projectService, ILogger<CreatePageController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        // GET: /CreatePage/
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index()-metoden anropades.");

            var nextProjectNumber = await _projectService.GetNextProjectNumberAsync();
            _logger.LogInformation($"Nästa projektnummer: {nextProjectNumber}");

            var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
            var services = await _projectService.GetAllServicesAsync();
            var customers = await _projectService.GetAllCustomersAsync();

            var model = new ProjectCreateViewModel
            {
                ProjectNumber = nextProjectNumber,
                StartDate = DateTime.Today,
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
                Orders = new List<OrderViewModel>(),
                Summary = new SummaryViewModel()
            };

            _logger.LogInformation("ViewModel för Index skapad och returneras.");
            return View(model);
        }

        // POST: /CreatePage/Save
        [HttpPost]
        public async Task<IActionResult> Save(ProjectCreateViewModel model)
        {
            _logger.LogInformation("Save()-metoden anropades.");
            _logger.LogInformation($"Mottagna värden -> TotalHours: {model.Summary?.TotalHours}, TotalPrice: {model.Summary?.TotalPrice}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState är ogiltig. Här är felen:");
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        _logger.LogWarning($"Fält: {error.Key}, Fel: {subError.ErrorMessage}");
                    }
                }

                var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
                var services = await _projectService.GetAllServicesAsync();
                var customers = await _projectService.GetAllCustomersAsync();

                model.ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    Name = pl.Name
                }).ToList();

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

                _logger.LogWarning("Returnerar vy med ogiltig ModelState.");
                return View("Index", model);
            }

            _logger.LogInformation("ModelState är giltig. Skapar ProjectDTO.");

            var projectDto = new ProjectDTO
            {
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
                    Price = o.Price
                }).ToList(),
                Summary = new SummaryDTO
                {
                    TotalHours = (int)(model.Summary?.TotalHours ?? 0), // Omvandlar till heltal
                    TotalPrice = model.Summary?.TotalPrice ?? 0,
                    Notes = model.Summary?.Notes
                }
            };

            _logger.LogInformation($"DTO skapad -> TotalHours: {projectDto.Summary.TotalHours}, TotalPrice: {projectDto.Summary.TotalPrice}");

            await _projectService.CreateProjectAsync(projectDto);

            _logger.LogInformation("Projektet har sparats i databasen. Omdirigerar till Home.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            _logger.LogInformation("Cancel()-metoden anropades. Omdirigerar till Home.");
            return RedirectToAction("Index", "Home");
        }
    }
}
