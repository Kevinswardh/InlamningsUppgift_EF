using Business.DTOs;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Factories
{
    /// <summary>
    /// Provides methods to create <see cref="Project"/> entities from data transfer objects.
    /// </summary>

    public static class ProjectFactory
    {
        /// <summary>
        /// Creates a <see cref="Project"/> entity from the provided <see cref="ProjectDTO"/> and associated <see cref="ProjectLeader"/>.
        /// </summary>
        /// <param name="dto">The data transfer object containing project details.</param>
        /// <param name="projectLeader">The project leader assigned to the project.</param>
        /// <returns>A new <see cref="Project"/> entity with populated properties.</returns>
        /// <exception cref="ArgumentException">Thrown when the end date is earlier than the start date.</exception>

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
