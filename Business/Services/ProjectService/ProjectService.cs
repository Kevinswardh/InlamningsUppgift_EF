using Business.DTOs;
using Business.Factories;
using Business.Services;
using Data.Database;
using Data.DatabaseRepository;
using Data.Entities;
using Microsoft.Extensions.Logging;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBaseRepository<ProjectLeader> _projectLeaderRepository;
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly AppDbContext _context;
    private readonly ILogger<ProjectService> _logger;
    private readonly IServiceService _service;

    public ProjectService(
        IProjectRepository projectRepository,
        IBaseRepository<ProjectLeader> projectLeaderRepository,
        IBaseRepository<Order> orderRepository,
        AppDbContext context,
        ILogger<ProjectService> logger,
        IServiceService serviceService)
    {
        _projectRepository = projectRepository;
        _projectLeaderRepository = projectLeaderRepository;
        _orderRepository = orderRepository;
        _context = context;
        _logger = logger;
        _service = serviceService;
    }

    public async Task<string> GetNextProjectNumberAsync()
    {
        var maxProjectNumber = await _projectRepository.GetMaxProjectNumberAsync();
        var numberPart = int.Parse(maxProjectNumber.Split('-')[1]);
        return $"P-{numberPart + 1}";
    }

    public async Task CreateProjectAsync(ProjectDTO projectDto)
    {
        await _projectRepository.BeginTransactionAsync(); // 🟢 Starta transaktion
        try
        {
            var projectLeader = await _projectLeaderRepository.GetSingleAsync(pl => pl.ProjectLeaderID == projectDto.ProjectLeaderID);
            if (projectLeader == null)
                throw new KeyNotFoundException("Projektledaren hittades inte.");

            var project = ProjectFactory.Create(projectDto, projectLeader);
            await _projectRepository.AddAsync(project);

            await _projectRepository.CommitTransactionAsync(); // ✅ Bekräfta transaktionen
        }
        catch
        {
            await _projectRepository.RollbackTransactionAsync(); // 🔴 Återställ vid fel
            throw;
        }
    }

    public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(p => new ProjectDTO
        {
            ProjectID = p.ProjectID,
            ProjectNumber = p.ProjectNumber,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Status = p.Status,
            ProjectLeaderID = p.ProjectLeaderID,
            ProjectLeaderFirstName = p.ProjectLeader?.FirstName ?? "Ej tilldelad",
            ProjectLeaderLastName = p.ProjectLeader?.LastName ?? "",

            Orders = p.Orders?.Select(o => new OrderDTO
            {
                CustomerID = o.CustomerID,
                ServiceID = o.ServiceID,
                ProjectID = o.ProjectID,
                Hours = o.Hours,
                Price = o.Price
            }).ToList() ?? new List<OrderDTO>(),

            // 🟢 Lägg till Summary om den finns
            Summary = p.Summary != null ? new SummaryDTO
            {
                SummaryID = p.Summary.SummaryID,
                ProjectID = p.Summary.ProjectID,
                TotalHours = p.Summary.TotalHours ?? 0,
                TotalPrice = p.Summary.TotalPrice ?? 0m,
                Notes = p.Summary.Notes ?? "Inga anteckningar"
            } : new SummaryDTO()
        }).ToList();
    }

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
            ProjectLeaderFirstName = project.ProjectLeader?.FirstName ?? "Ej tilldelad",
            ProjectLeaderLastName = project.ProjectLeader?.LastName ?? "",

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

            // 🟢 Lägger till Summary om den finns
            Summary = project.Summary != null ? new SummaryDTO
            {
                SummaryID = project.Summary.SummaryID,
                ProjectID = project.Summary.ProjectID,
                TotalHours = project.Summary.TotalHours ?? 0,  // ✅ Om null, sätt till 0
                TotalPrice = project.Summary.TotalPrice ?? 0m, // ✅ Om null, sätt till 0
                Notes = project.Summary.Notes ?? "Inga anteckningar"
            } : new SummaryDTO()
        };
    }


    public async Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync()
    {
        var leaders = await _projectLeaderRepository.GetAllAsync();
        return leaders.Select(l => new ProjectLeaderDTO
        {
            ProjectLeaderID = l.ProjectLeaderID,
            FirstName = l.FirstName, // ✅ Ny kod
            LastName = l.LastName,   // ✅ Ny kod
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

    public async Task UpdateProjectAsync(ProjectDTO projectDto)
    {
        await _projectRepository.BeginTransactionAsync();
        try
        {
            var project = await _projectRepository.GetProjectByIdWithDetailsAsync(projectDto.ProjectID);
            if (project == null)
                throw new KeyNotFoundException("Projektet hittades inte.");

            // Uppdatera grundläggande projektuppgifter
            project.ProjectNumber = projectDto.ProjectNumber;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;
            project.EndDate = projectDto.EndDate;
            project.Status = projectDto.Status;
            project.ProjectLeaderID = projectDto.ProjectLeaderID;

            // Uppdatera Orders
            project.Orders.Clear();
            foreach (var orderDto in projectDto.Orders)
            {
                var service = await _service.GetServiceEntityByIdAsync(orderDto.ServiceID);
                project.Orders.Add(new Order
                {
                    CustomerID = orderDto.CustomerID,
                    ServiceID = orderDto.ServiceID,
                    ProjectID = projectDto.ProjectID,
                    Hours = orderDto.Hours,
                    Price = orderDto.Price,
                    Service = service // Hämtar rätt Service-objekt
                });
            }

            // Uppdatera Summary om det finns
            if (projectDto.Summary != null)
            {
                project.Summary ??= new Summary();
                project.Summary.TotalHours = projectDto.Summary.TotalHours;
                project.Summary.TotalPrice = projectDto.Summary.TotalPrice;
                project.Summary.Notes = projectDto.Summary.Notes;
            }

            await _projectRepository.UpdateAsync(project);
            await _projectRepository.CommitTransactionAsync();
        }
        catch
        {
            await _projectRepository.RollbackTransactionAsync();
            throw;
        }
    }


    public async Task DeleteProjectAsync(int id)
    {
        await _projectRepository.BeginTransactionAsync();
        try
        {
            var project = await _projectRepository.GetSingleAsync(p => p.ProjectID == id);
            if (project == null)
                throw new KeyNotFoundException("Projektet hittades inte.");

            await _projectRepository.DeleteAsync(project);
            await _projectRepository.CommitTransactionAsync();
        }
        catch
        {
            await _projectRepository.RollbackTransactionAsync();
            throw;
        }
    }
}
