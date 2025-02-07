using Business.DTOs;
using Data.Entities;
using System;
using System.Collections.Generic;

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

            return new Project
            {
                ProjectID = dto.ProjectID,
                ProjectNumber = dto.ProjectNumber,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Planerat" : dto.Status,
                ProjectLeaderID = projectLeader?.ProjectLeaderID ?? 0,
                ProjectLeader = projectLeader,
                Orders = new List<Order>(), // Kan mappas senare om nödvändigt
                Summary = dto.Summary != null
                    ? new Summary
                    {
                        TotalHours = dto.Summary.TotalHours,
                        TotalPrice = dto.Summary.TotalPrice,
                        Notes = dto.Summary.Notes
                    }
                    : null
            };
        }

    }
}
