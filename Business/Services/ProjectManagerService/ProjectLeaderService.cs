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
        private readonly IProjectRepository _projectRepository;
        public ProjectLeaderService(IBaseRepository<ProjectLeader> projectLeaderRepository, IProjectRepository projectRepository)
        {
            _projectLeaderRepository = projectLeaderRepository;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectLeaderDTO>> GetAllProjectLeadersAsync()
        {
            var leaders = await _projectLeaderRepository.GetAllAsync();
            return leaders.Select(pl => new ProjectLeaderDTO
            {
                ProjectLeaderID = pl.ProjectLeaderID,
                FirstName = pl.FirstName,  // ✅ Hämtar FirstName
                LastName = pl.LastName,    // ✅ Hämtar LastName
                Email = pl.Email,
                Phone = pl.Phone,
                Department = pl.Department,
                IsDeleted = pl.IsDeleted  // Lägg till IsDeleted här
            }).ToList();
        }



        public async Task CreateProjectLeaderAsync(ProjectLeaderDTO projectLeaderDto)
        {
            await _projectLeaderRepository.BeginTransactionAsync();
            try
            {
                var projectLeader = ProjectLeaderFactory.Create(
                    projectLeaderDto.FirstName,  // ✅ Använd FirstName
                    projectLeaderDto.LastName,   // ✅ Använd LastName
                    projectLeaderDto.Email,
                    projectLeaderDto.Phone,
                    projectLeaderDto.Department
                );

                await _projectLeaderRepository.AddAsync(projectLeader);
                await _projectLeaderRepository.CommitTransactionAsync();
            }
            catch
            {
                await _projectLeaderRepository.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task DeleteProjectLeaderAsync(int projectLeaderId)
        {
            await _projectLeaderRepository.BeginTransactionAsync(); // Starta transaktion
            try
            {
                // 🔹 Hämta alla projekt som är kopplade till projektledaren
                var projects = await _projectRepository.GetProjectsByLeaderIdAsync(projectLeaderId);

                // Loop igenom projekten och uppdatera ProjectLeaderID
                foreach (var project in projects)
                {
                    project.ProjectLeaderID = -1; // Eller sätt till en standardprojektledare
                    await _projectRepository.UpdateAsync(project);
                }

                // 🔹 Hämta projektledaren från databasen
                var leader = await _projectLeaderRepository.GetSingleAsync(pl => pl.ProjectLeaderID == projectLeaderId);
                if (leader != null)
                {
                    leader.IsDeleted = 1; // Markera som borttagen
                    await _projectLeaderRepository.UpdateAsync(leader);
                }

                // Bekräfta transaktionen
                await _projectLeaderRepository.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                // Om något går fel, återställ ändringar
                await _projectLeaderRepository.RollbackTransactionAsync();
                throw new Exception("Ett fel uppstod vid borttagning av projektledare.", ex);
            }
        }




    }
}
