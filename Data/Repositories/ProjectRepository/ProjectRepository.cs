﻿using Data.Database;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context) { }



        //Overrides
        public override async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.ProjectLeader)
                .Include(p => p.Orders)
                    .ThenInclude(o => o.Customer)
                .Include(p => p.Orders)
                    .ThenInclude(o => o.Service)
                .Include(p => p.Summary)
                .ToListAsync();
        }


        //New (Not base)

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
        /*

      // SQL RAW / Dapper versioner av metoderna

      public async Task<IEnumerable<Project>> GetAllAsync()
      {
          var sql = @"
              SELECT * FROM Projects p
              LEFT JOIN ProjectLeaders pl ON p.ProjectLeaderId = pl.Id
              LEFT JOIN Orders o ON o.ProjectId = p.ProjectID
              LEFT JOIN Customers c ON o.CustomerId = c.Id
              LEFT JOIN Services s ON o.ServiceId = s.Id
              LEFT JOIN Summaries sm ON sm.ProjectId = p.ProjectID";

          using var connection = _context.Database.GetDbConnection();
          return await connection.QueryAsync<Project, ProjectLeader, Order, Customer, Service, Summary, Project>(
              sql,
              (p, pl, o, c, s, sm) =>
              {
                  p.ProjectLeader = pl;
                  if (o != null)
                  {
                      o.Customer = c;
                      o.Service = s;
                      p.Orders ??= new List<Order>();
                      p.Orders.Add(o);
                  }
                  p.Summary = sm;
                  return p;
              },
              splitOn: "Id,ProjectID"
          );
      }

      public async Task<string> GetMaxProjectNumberAsync()
      {
          var sql = "SELECT TOP 1 ProjectNumber FROM Projects ORDER BY ProjectID DESC";
          using var connection = _context.Database.GetDbConnection();
          return await connection.QueryFirstOrDefaultAsync<string>(sql) ?? "P-0";
      }

      public async Task<IEnumerable<ProjectLeader>> GetAllProjectLeadersAsync()
      {
          var sql = "SELECT * FROM ProjectLeaders";
          using var connection = _context.Database.GetDbConnection();
          return await connection.QueryAsync<ProjectLeader>(sql);
      }

      public async Task<IEnumerable<Service>> GetAllServicesAsync()
      {
          var sql = "SELECT * FROM Services";
          using var connection = _context.Database.GetDbConnection();
          return await connection.QueryAsync<Service>(sql);
      }

      public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
      {
          var sql = "SELECT * FROM Customers";
          using var connection = _context.Database.GetDbConnection();
          return await connection.QueryAsync<Customer>(sql);
      }

      public async Task<Project> GetProjectByIdWithDetailsAsync(int id)
      {
          var sql = @"
              SELECT * FROM Projects p
              LEFT JOIN ProjectLeaders pl ON p.ProjectLeaderId = pl.Id
              LEFT JOIN Orders o ON o.ProjectId = p.ProjectID
              LEFT JOIN Customers c ON o.CustomerId = c.Id
              LEFT JOIN Services s ON o.ServiceId = s.Id
              LEFT JOIN Summaries sm ON sm.ProjectId = p.ProjectID
              WHERE p.ProjectID = @Id";

          using var connection = _context.Database.GetDbConnection();
          var projectDict = new Dictionary<int, Project>();

          var result = await connection.QueryAsync<Project, ProjectLeader, Order, Customer, Service, Summary, Project>(
              sql,
              (p, pl, o, c, s, sm) =>
              {
                  if (!projectDict.TryGetValue(p.ProjectID, out var project))
                  {
                      project = p;
                      project.ProjectLeader = pl;
                      project.Orders = new List<Order>();
                      projectDict[p.ProjectID] = project;
                  }

                  if (o != null)
                  {
                      o.Customer = c;
                      o.Service = s;
                      projectDict[p.ProjectID].Orders.Add(o);
                  }
                  project.Summary = sm;

                  return project;
              },
              new { Id = id },
              splitOn: "Id,ProjectID"
          );

          return projectDict.Values.FirstOrDefault() ?? throw new KeyNotFoundException($"Projekt med ID {id} hittades inte.");
      }

      public async Task DeleteOrderAsync(Order order)
      {
          var sql = "DELETE FROM Orders WHERE OrderID = @OrderID";
          using var connection = _context.Database.GetDbConnection();
          await connection.ExecuteAsync(sql, new { OrderID = order.OrderID });
      }

      */

    }
}

