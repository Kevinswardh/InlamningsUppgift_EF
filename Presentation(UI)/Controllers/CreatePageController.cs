﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly ICustomerService _customerService;
        private readonly IServiceService _service;

        public CreatePageController(IProjectService projectService, ILogger<CreatePageController> logger, ICustomerService customerService, IServiceService service)
        {
            _projectService = projectService;
            _logger = logger;
            _customerService = customerService;
            _service = service;
        }

        // GET: /CreatePage/
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

        [HttpPost]
        public async Task<IActionResult> Save(ProjectCreateViewModel model)
        {
            _logger.LogInformation("Save()-metoden anropades.");

            // 🔍 **Validering av projektinfo**
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                ModelState.AddModelError("Description", "Benämning är obligatorisk.");
            }

            if (model.StartDate == default)
            {
                ModelState.AddModelError("StartDate", "Startdatum är obligatoriskt.");
            }

            if (string.IsNullOrWhiteSpace(model.Status))
            {
                ModelState.AddModelError("Status", "Status är obligatoriskt.");
            }

            if (model.ProjectLeaderID == 0)
            {
                ModelState.AddModelError("ProjectLeaderID", "Välj en giltig projektledare.");
            }

            // 🔍 **Validering av beställningar**
            if (model.Orders == null || !model.Orders.Any())
            {
                ModelState.AddModelError("Orders", "Minst en beställning krävs.");
            }
            else
            {
                for (int i = 0; i < model.Orders.Count; i++)
                {
                    var order = model.Orders[i];

                    if (order.CustomerID == 0)
                    {
                        ModelState.AddModelError($"Orders[{i}].CustomerID", "Välj en giltig kund.");
                    }

                    if (order.ServiceID == 0)
                    {
                        ModelState.AddModelError($"Orders[{i}].ServiceID", "Välj en giltig tjänst.");
                    }

                    if (order.Hours <= 0)
                    {
                        ModelState.AddModelError($"Orders[{i}].Hours", "Timmar måste vara större än 0.");
                    }

                    if (order.Price <= 0)
                    {
                        ModelState.AddModelError($"Orders[{i}].Price", "Pris per timme måste vara större än 0.");
                    }
                }
            }

            // Om det finns valideringsfel, returnera vyn med ModelState
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState är ogiltig, returnerar vy med felmeddelanden.");

                // Ladda om listor för dropdown-menyer
                var projectLeaders = await _projectService.GetAllProjectLeadersAsync();
                var services = await _service.GetAllServicesAsync();
                var customers = await _customerService.GetAllCustomersAsync();

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

                return View("Index", model);
            }

            // ✅ **Om valideringen passerar, skapa DTO och spara i databasen**
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
                    TotalHours = (int)(model.Summary?.TotalHours ?? 0),
                    TotalPrice = model.Summary?.TotalPrice ?? 0,
                    Notes = model.Summary?.Notes
                }
            };

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
