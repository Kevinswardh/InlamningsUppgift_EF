using Business.DTOs;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Factories
{
    public static class ProjectFactory
    {
        public static Project Create(ProjectDTO dto, ProjectLeader projectLeader)
        {
            if (dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
            {
                throw new ArgumentException("Slutdatum kan inte vara före startdatum.");
            }

            var project = new Project
            {
                ProjectID = dto.ProjectID,
                ProjectNumber = dto.ProjectNumber,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Planerat" : dto.Status,
                ProjectLeaderID = projectLeader?.ProjectLeaderID ?? 0,
                ProjectLeader = projectLeader,

                // ✅ Lägg till Orders direkt i projektet
                Orders = dto.Orders.Select(o => new Order
                {
                    CustomerID = o.CustomerID,
                    ServiceID = o.ServiceID,
                    ProjectID = dto.ProjectID, // 👈 Viktigt att koppla rätt ProjectID!
                    Hours = o.Hours,
                    Price = o.Price
                }).ToList(),

                // 🔹 Lägg till Summary om det finns
                Summary = dto.Summary != null
                    ? new Summary
                    {
                        TotalHours = dto.Summary.TotalHours,
                        TotalPrice = dto.Summary.TotalPrice,
                        Notes = dto.Summary.Notes
                    }
                    : null
            };

            return project;
        }
    }
}
