using Data.Entities;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    public interface IProjectRepository : IBaseRepository<Project>
    {
        Task<string> GetMaxProjectNumberAsync();
        Task<IEnumerable<ProjectLeader>> GetAllProjectLeadersAsync();

        Task<IEnumerable<Service>> GetAllServicesAsync();

        Task<IEnumerable<Customer>> GetAllCustomersAsync();
    }
}
