using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Presentation_UI_.ViewModels;
using Business.DTOs;

namespace Presentation_UI_.Controllers
{
    public class CreatePageController : Controller
    {
        private readonly IProjectService _projectService;

        public CreatePageController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: /CreatePage/
        public async Task<IActionResult> Index()
        {
            // Hämta nästa projektnummer från tjänsten
            var nextProjectNumber = await _projectService.GetNextProjectNumberAsync();

            // Hämta alla projektledare
            var projectLeaders = await _projectService.GetAllProjectLeadersAsync();

            // Hämta alla tjänster
            var services = await _projectService.GetAllServicesAsync();

            // Hämta alla kunder
            var customers = await _projectService.GetAllCustomersAsync();

            // Skapa och fyll ViewModel
            var model = new ProjectViewModel
            {
                ProjectNumber = nextProjectNumber,
                ProjectLeaders = projectLeaders.Select(pl => new ProjectLeaderViewModel
                {
                    ProjectLeaderID = pl.ProjectLeaderID,
                    Name = pl.Name
                }).ToList(),
               Orders = services.SelectMany(service =>
                  customers.Select(customer => new OrderViewModel
                      {
                          Service = new ServiceViewModel
                          {
                              ServiceID = service.ServiceID,
                              ServiceName = service.ServiceName
                          },
                          Customer = new CustomerViewModel
                          {
                              CustomerID = customer.CustomerID,
                              CustomerName = customer.CustomerName
                          }
                      })
                ).ToList()
            };

            return View(model);
        }


        // POST: /CreatePage/Save
        [HttpPost]
        public async Task<IActionResult> Save(ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Omvandla ViewModel till DTO
                var projectDto = new ProjectDTO
                {
                    ProjectNumber = model.ProjectNumber, // Använd det förifyllda projektnumret
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
                        TotalHours = model.Summary.TotalHours,
                        TotalPrice = model.Summary.TotalPrice,
                        Notes = model.Summary.Notes
                    }
                };

                // Skicka DTO till tjänsten
                await _projectService.CreateProjectAsync(projectDto);

                // Redirect till "Home" efter att projektet skapats
                return RedirectToAction("Index", "Home");
            }

            // Om ModelState inte är giltig, visa vyn igen med de aktuella fälten
            return View("Index", model);
        }
    }
}
