using Data.Database;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context) { }

        public async Task<string> GetMaxProjectNumberAsync()
        {
            var lastProject = await _dbSet.OrderByDescending(p => p.ProjectID).FirstOrDefaultAsync();
            return lastProject?.ProjectNumber ?? "P-0";
        }
        public async Task<IEnumerable<ProjectLeader>> GetAllProjectLeadersAsync()
        {
            return await _context.ProjectLeaders.ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetAllProjectsWithDetailsAsync()
        {
            return await _dbSet
                .Include(p => p.ProjectLeader) // Inkluderar ProjectLeader
                .Include(p => p.Orders)
                    .ThenInclude(o => o.Customer)
                .Include(p => p.Orders)
                    .ThenInclude(o => o.Service)
                .Include(p => p.Summary) // Inkluderar Summary
                .ToListAsync();
        }
        public async Task<Project> GetProjectByIdWithDetailsAsync(int id)
        {
            var project = await _dbSet
                .Where(p => p.ProjectID == id)
                .Include(p => p.ProjectLeader)
                .Include(p => p.Orders)
                    .ThenInclude(o => o.Customer)
                .Include(p => p.Orders)
                    .ThenInclude(o => o.Service)
                .Include(p => p.Summary)
                .FirstOrDefaultAsync();

            if (project == null)
            {
                throw new KeyNotFoundException($"Projekt med ID {id} hittades inte.");
            }

            return project;
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }






        /* /// <summary>
         /// Hämtar senaste projektnumret med SQL istället för LINQ.
         /// </summary>
         public async Task<string> GetMaxProjectNumberAsync()
         {
             var lastProject = await _context.Projects
                 .FromSqlRaw("SELECT TOP 1 * FROM Projects ORDER BY ProjectID DESC")
                 .FirstOrDefaultAsync();

             return lastProject?.ProjectNumber ?? "P-0";
         }

         /// <summary>
         /// Hämtar alla projektledare med en SQL-query.
         /// </summary>
         public async Task<IEnumerable<ProjectLeader>> GetAllProjectLeadersAsync()
         {
             return await _context.ProjectLeaders
                 .FromSqlRaw("SELECT * FROM ProjectLeaders")
                 .ToListAsync();
         }*/
    }
}

