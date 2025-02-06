using Business.DTOs;
using Business.Factories;
using Data.DatabaseRepository;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IBaseRepository<ProjectLeader> _projectLeaderRepository;

        public ProjectService(IProjectRepository projectRepository, IBaseRepository<ProjectLeader> projectLeaderRepository)
        {
            _projectRepository = projectRepository;
            _projectLeaderRepository = projectLeaderRepository;
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
            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(p => new ProjectDTO
            {
                ProjectID = p.ProjectID,
                ProjectNumber = p.ProjectNumber,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                ProjectLeaderID = p.ProjectLeaderID
            }).ToList();
        }

        /// <summary>
        /// Hämtar ett projekt baserat på dess ID.
        /// </summary>
        public async Task<ProjectDTO> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetSingleAsync(p => p.ProjectID == id);
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
                ProjectLeaderID = project.ProjectLeaderID
            };
        }

        /// <summary>
        /// Uppdaterar ett befintligt projekt.
        /// </summary>
        public async Task UpdateProjectAsync(ProjectDTO projectDto)
        {
            var project = await _projectRepository.GetSingleAsync(p => p.ProjectID == projectDto.ProjectID);
            if (project == null)
                throw new KeyNotFoundException("Projektet hittades inte.");

            project.ProjectNumber = projectDto.ProjectNumber;
            project.Description = projectDto.Description;
            project.StartDate = projectDto.StartDate;
            project.EndDate = projectDto.EndDate;
            project.Status = projectDto.Status;
            project.ProjectLeaderID = projectDto.ProjectLeaderID;

            await _projectRepository.UpdateAsync(project);
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
