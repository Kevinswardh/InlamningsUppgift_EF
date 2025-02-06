using Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Skapar ett nytt projekt med relationer.
        /// </summary>
        Task CreateProjectAsync(ProjectDTO projectDto);

        /// <summary>
        /// Hämtar alla projekt.
        /// </summary>
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();

        /// <summary>
        /// Hämtar ett projekt baserat på dess ID.
        /// </summary>
        Task<ProjectDTO> GetProjectByIdAsync(int id);

        /// <summary>
        /// Uppdaterar ett befintligt projekt.
        /// </summary>
        Task UpdateProjectAsync(ProjectDTO projectDto);

        /// <summary>
        /// Tar bort ett projekt baserat på dess ID.
        /// </summary>
        Task DeleteProjectAsync(int id);

        /// <summary>
        /// Hämtar alla projektledare.
        /// </summary>
        Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync();

        /// <summary>
        /// Hämtar nästa tillgängliga projektnummer.
        /// </summary>
        Task<string> GetNextProjectNumberAsync();

        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();

        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
    }
}
