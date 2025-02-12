using Data.Entities;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        //Overrides
        Task<IEnumerable<Project>> GetAllAsync();



        //New(not from base)
        Task<string> GetMaxProjectNumberAsync();
        Task<IEnumerable<ProjectLeader>> GetAllProjectLeadersAsync();

        Task<IEnumerable<Service>> GetAllServicesAsync();

        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        Task<Project> GetProjectByIdWithDetailsAsync(int id);
        Task DeleteOrderAsync(Order order);
    }
}
