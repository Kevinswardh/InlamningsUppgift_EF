﻿using Business.DTOs;
using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();
        Task CreateServiceAsync(ServiceDTO serviceDto);
        Task DeleteServiceAsync(int serviceId);
        Task<Service> GetServiceEntityByIdAsync(int serviceId);
    }
}
