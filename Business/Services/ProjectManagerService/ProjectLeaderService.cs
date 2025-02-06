using Business.DTOs;
using Business.Factories;
using Data.DatabaseRepository;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProjectLeaderService : IProjectLeaderService
    {
        private readonly IBaseRepository<ProjectLeader> _projectLeaderRepository;

        public ProjectLeaderService(IBaseRepository<ProjectLeader> projectLeaderRepository)
        {
            _projectLeaderRepository = projectLeaderRepository;
        }

        public async Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync()
        {
            var leaders = await _projectLeaderRepository.GetAllAsync();
            return leaders.Select(pl => new ProjectLeaderDTO
            {
                ProjectLeaderID = pl.ProjectLeaderID,
                Name = pl.Name,
                Email = pl.Email,
                Phone = pl.Phone,
                Department = pl.Department
            }).ToList();
        }

        public async Task CreateProjectLeaderAsync(ProjectLeaderDTO projectLeaderDto)
        {
            var projectLeader = ProjectLeaderFactory.Create(
                projectLeaderDto.Name,
                projectLeaderDto.Email,
                projectLeaderDto.Phone,
                projectLeaderDto.Department
            );

            await _projectLeaderRepository.AddAsync(projectLeader);
        }

        public async Task DeleteProjectLeaderAsync(int projectLeaderId)
        {
            var leader = await _projectLeaderRepository.GetSingleAsync(pl => pl.ProjectLeaderID == projectLeaderId);
            if (leader != null)
            {
                await _projectLeaderRepository.DeleteAsync(leader);
            }
        }
    }
}
