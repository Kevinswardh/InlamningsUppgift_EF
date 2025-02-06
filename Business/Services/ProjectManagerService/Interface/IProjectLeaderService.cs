using Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IProjectLeaderService
    {
        Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync();
        Task CreateProjectLeaderAsync(ProjectLeaderDTO projectLeaderDto);
        Task DeleteProjectLeaderAsync(int projectLeaderId);
    }
}
