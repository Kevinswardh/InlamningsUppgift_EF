using Data.Entities;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    /// <summary>
    /// Defines methods for managing project data, including CRUD operations and project-specific queries.
    /// </summary>
    public interface IProjectRepository : IBaseRepository<Project>
    {
        //Overrides

        /// <summary>
        /// Retrieves all projects with their related data (project leaders, orders, customers, services, and summaries) asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="Project"/> entities with detailed information.</returns>
        Task<IEnumerable<Project>> GetAllAsync();



        //No overrides(new methods only in ProjectRepository)

        /// <summary>
        /// Retrieves the highest existing project number asynchronously.
        /// </summary>
        /// <returns>A string representing the highest project number, or "P-0" if none exists.</returns>
        Task<string> GetMaxProjectNumberAsync();

        /// <summary>
        /// Retrieves all project leaders asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="ProjectLeader"/> entities.</returns>
        Task<IEnumerable<ProjectLeader>> GetAllProjectLeadersAsync();


        /// <summary>
        /// Retrieves all services asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="Service"/> entities.</returns>
        Task<IEnumerable<Service>> GetAllServicesAsync();


        /// <summary>
        /// Retrieves all customers asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="Customer"/> entities.</returns>
        Task<IEnumerable<Customer>> GetAllCustomersAsync();


        /// <summary>
        /// Retrieves a specific project by ID with related data, including project leader, orders, services, customers, and summary.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <returns>A <see cref="Project"/> entity with detailed information.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the project is not found.</exception>
        Task<Project> GetProjectByIdWithDetailsAsync(int id);


        /// <summary>
        /// Retrieves all projects assigned to a specific project leader asynchronously.
        /// </summary>
        /// <param name="projectLeaderId">The ID of the project leader.</param>
        /// <returns>A list of <see cref="Project"/> entities led by the specified leader.</returns>
        Task<List<Project>> GetProjectsByLeaderIdAsync(int projectLeaderId);


        /// <summary>
        /// Deletes a specific order from the database asynchronously.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> entity to delete.</param>
        Task DeleteOrderAsync(Order order);
    }
}
