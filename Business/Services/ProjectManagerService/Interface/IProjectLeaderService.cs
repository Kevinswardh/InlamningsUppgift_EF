using Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Defines methods for managing project leader operations.
    /// </summary>
    public interface IProjectLeaderService
    {
        /// <summary>
        /// Retrieves all project leaders asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="ProjectLeaderDTO"/> representing all project leaders.</returns>
        Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync();


        /// <summary>
        /// Creates a new project leader asynchronously.
        /// </summary>
        /// <param name="projectLeaderDto">The data transfer object containing project leader information.</param>
        Task CreateProjectLeaderAsync(ProjectLeaderDTO projectLeaderDto);


        /// <summary>
        /// Deletes a project leader asynchronously by their ID, updating related projects.
        /// </summary>
        /// <param name="projectLeaderId">The unique identifier of the project leader to be deleted.</param>
        Task DeleteProjectLeaderAsync(int projectLeaderId);
    }
}
