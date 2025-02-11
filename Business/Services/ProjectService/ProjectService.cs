using Business.DTOs;
using Business.Factories;
using Data.DatabaseRepository;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IBaseRepository<ProjectLeader> _projectLeaderRepository;
        private readonly ILogger<ProjectService> _logger;
        public ProjectService(IProjectRepository projectRepository, IBaseRepository<ProjectLeader> projectLeaderRepository, ILogger<ProjectService> logger)
        {
            _projectRepository = projectRepository;
            _projectLeaderRepository = projectLeaderRepository;
            _logger = logger;
        }

        /// <summary>
        /// Hämtar nästa tillgängliga projektnummer.
        /// </summary>
        public async Task<string> GetNextProjectNumberAsync()
        {
            var maxProjectNumber = await _projectRepository.GetMaxProjectNumberAsync();
            var numberPart = int.Parse(maxProjectNumber.Split('-')[1]);
            return $"P-{numberPart + 1}";
        }

        /// <summary>
        /// Skapar ett nytt projekt baserat på DTO:n.
        /// </summary>
        public async Task CreateProjectAsync(ProjectDTO projectDto)
        {
            var projectLeader = await _projectLeaderRepository.GetSingleAsync(pl => pl.ProjectLeaderID == projectDto.ProjectLeaderID);
            if (projectLeader == null)
                throw new KeyNotFoundException("Projektledaren hittades inte.");

            var project = ProjectFactory.Create(projectDto, projectLeader);
            await _projectRepository.AddAsync(project);
        }

        /// <summary>
        /// Hämtar alla projekt som en lista av DTO:er.
        /// </summary>
        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllProjectsWithDetailsAsync();

            return projects.Select(p => new ProjectDTO
            {
                ProjectID = p.ProjectID,
                ProjectNumber = p.ProjectNumber,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                ProjectLeaderID = p.ProjectLeaderID,
                ProjectLeaderName = p.ProjectLeader?.Name ?? "Ej tilldelad", // Lägg till detta
                Summary = p.Summary != null ? new SummaryDTO
                {
                    SummaryID = p.Summary.SummaryID,
                    ProjectID = p.Summary.ProjectID,
                    TotalHours = p.Summary.TotalHours ?? 0,
                    TotalPrice = p.Summary.TotalPrice ?? 0m,
                    Notes = p.Summary.Notes
                } : null,
                Orders = p.Orders?.Select(o => new OrderDTO
                {
                    CustomerID = o.CustomerID,
                    ServiceID = o.ServiceID,
                    ProjectID = o.ProjectID,
                    Hours = o.Hours,
                    Price = o.Price
                }).ToList() ?? new List<OrderDTO>()
            }).ToList();
        }







        /// <summary>
        /// Hämtar ett projekt baserat på dess ID från ProjectRepository.
        /// </summary>
        public async Task<ProjectDTO> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetProjectByIdWithDetailsAsync(id);

            if (project == null)
                throw new KeyNotFoundException("Projektet hittades inte.");

            return new ProjectDTO
            {
                ProjectID = project.ProjectID,
                ProjectNumber = project.ProjectNumber,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                ProjectLeaderID = project.ProjectLeaderID,
                ProjectLeaderName = project.ProjectLeader?.Name ?? "Ej tilldelad",

                Orders = project.Orders.Select(o => new OrderDTO
                {
                    ProjectID = o.ProjectID,
                    CustomerID = o.CustomerID,
                    ServiceID = o.ServiceID,
                    CustomerName = o.Customer.CustomerName ?? "Okänd kund",
                    ServiceName = o.Service.ServiceName ?? "Okänd tjänst",
                    Hours = o.Hours,
                    Price = o.Price
                }).ToList(),
                Summary = project.Summary != null ? new SummaryDTO
                {
                    TotalHours = project.Summary.TotalHours ?? 0m,  // ✅ Om null, sätt till 0
                    TotalPrice = project.Summary.TotalPrice ?? 0m,  // ✅ Om null, sätt till 0
                    Notes = project.Summary.Notes ?? "Inga anteckningar"
                } : new SummaryDTO(),

            };
        }

        /// <summary>
        /// Uppdaterar ett befintligt projekt.
        /// </summary>
        public async Task UpdateProjectAsync(ProjectDTO projectDto)
        {
            // Hämta projektet från databasen baserat på ID och inkludera Orders och deras Services
            var project = await _projectRepository.GetProjectByIdWithDetailsAsync(projectDto.ProjectID);

            if (project == null)
            {
                _logger.LogError("Projekt med ID: {ProjectId} hittades inte.", projectDto.ProjectID);
                throw new KeyNotFoundException("Projektet hittades inte.");
            }

            // Logga innan uppdatering
            _logger.LogInformation("Uppdaterar projekt med ID: {ProjectId} med nya värden.", projectDto.ProjectID);

            // Mappa in projektets data från DTO:n
            project.ProjectNumber = projectDto.ProjectNumber;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;
            project.EndDate = projectDto.EndDate;
            project.Status = projectDto.Status;
            project.ProjectLeaderID = projectDto.ProjectLeaderID;

            // 🔴 **Steg 1: Hitta ordrar som ska tas bort**
            var ordersToDelete = project.Orders
                .Where(existingOrder => !projectDto.Orders.Any(o =>
                    o.CustomerID == existingOrder.CustomerID &&
                    o.ServiceID == existingOrder.ServiceID))
                .ToList();

            foreach (var order in ordersToDelete)
            {
                _logger.LogInformation("Tar bort order med ServiceID: {ServiceID} och CustomerID: {CustomerID}", order.ServiceID, order.CustomerID);
                await _projectRepository.DeleteOrderAsync(order); // ✅ Ny metod i ProjectRepository
            }

            // 🟢 **Steg 2: Uppdatera befintliga ordrar**
            foreach (var orderDto in projectDto.Orders)
            {
                var order = project.Orders.FirstOrDefault(o => o.CustomerID == orderDto.CustomerID && o.ServiceID == orderDto.ServiceID);

                if (order != null)
                {
                    // Om ordern finns, uppdatera
                    order.Hours = orderDto.Hours;
                    order.Price = orderDto.Price;

                    // ✅ Uppdatera ServiceName från navigationspropertyn (om den redan är laddad)
                    if (order.Service != null)
                    {
                        order.Service.ServiceName = orderDto.ServiceName;
                    }
                }
                else
                {
                    // 🟢 **Steg 3: Skapa en ny order och koppla rätt Service-objekt**
                    var service = project.Orders.Select(o => o.Service).FirstOrDefault(s => s.ServiceID == orderDto.ServiceID)
                          ?? (await _projectRepository.GetAllServicesAsync()).FirstOrDefault(s => s.ServiceID == orderDto.ServiceID);

                    if (service == null)
                    {
                        _logger.LogError("Service med ID {ServiceID} hittades inte.", orderDto.ServiceID);
                        throw new KeyNotFoundException($"Service med ID {orderDto.ServiceID} hittades inte.");
                    }

                    var newOrder = new Order
                    {
                        ProjectID = project.ProjectID,
                        CustomerID = orderDto.CustomerID,
                        ServiceID = orderDto.ServiceID,
                        Hours = orderDto.Hours,
                        Price = orderDto.Price,
                        Service = service // ✅ Koppla rätt Service-objekt
                    };

                    project.Orders.Add(newOrder);
                }
            }

            // 🔵 **Steg 4: Uppdatera Summary (om det finns några förändringar)**
            if (projectDto.Summary != null)
            {
                if (project.Summary != null)
                {
                    project.Summary.TotalHours = projectDto.Summary.TotalHours;
                    project.Summary.TotalPrice = projectDto.Summary.TotalPrice;
                    project.Summary.Notes = projectDto.Summary.Notes;
                }
                else
                {
                    project.Summary = new Summary
                    {
                        ProjectID = project.ProjectID,
                        TotalHours = projectDto.Summary.TotalHours,
                        TotalPrice = projectDto.Summary.TotalPrice,
                        Notes = projectDto.Summary.Notes
                    };
                }
            }

            // 🔥 **Spara uppdateringar**
            await _projectRepository.UpdateAsync(project);

            // Logga efter uppdatering
            _logger.LogInformation("Projekt med ID: {ProjectId} uppdaterades framgångsrikt.", projectDto.ProjectID);
        }




        /// <summary>
        /// Tar bort ett projekt baserat på dess ID.
        /// </summary>
        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetSingleAsync(p => p.ProjectID == id);
            if (project == null)
                throw new KeyNotFoundException("Projektet hittades inte.");

            await _projectRepository.DeleteAsync(project);
        }

        /// <summary>
        /// Hämtar alla projektledare som en lista av DTO:er.
        /// </summary>
        public async Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync()
        {
            var leaders = await _projectLeaderRepository.GetAllAsync();
            return leaders.Select(l => new ProjectLeaderDTO
            {
                ProjectLeaderID = l.ProjectLeaderID,
                Name = l.Name,
                Email = l.Email,
                Phone = l.Phone,
                Department = l.Department
            }).ToList();
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            var services = await _projectRepository.GetAllServicesAsync();
            return services.Select(s => new ServiceDTO
            {
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName
            }).ToList();
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _projectRepository.GetAllCustomersAsync();
            return customers.Select(c => new CustomerDTO
            {
                CustomerID = c.CustomerID,
                CustomerName = c.CustomerName
            }).ToList();
        }

    }
}
