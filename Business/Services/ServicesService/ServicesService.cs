using Business.DTOs;
using Data.DatabaseRepository;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IBaseRepository<Service> _serviceRepository;

        public ServiceService(IBaseRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return services.Select(s => new ServiceDTO
            {
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName
            }).ToList();
        }

        public async Task CreateServiceAsync(ServiceDTO serviceDto)
        {
            await _serviceRepository.BeginTransactionAsync();
            try
            {
                var service = new Service { ServiceName = serviceDto.ServiceName };
                await _serviceRepository.AddAsync(service);
                await _serviceRepository.CommitTransactionAsync();
            }
            catch
            {
                await _serviceRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteServiceAsync(int serviceId)
        {
            await _serviceRepository.BeginTransactionAsync();
            try
            {
                var service = await _serviceRepository.GetSingleAsync(s => s.ServiceID == serviceId);
                if (service != null)
                {
                    await _serviceRepository.DeleteAsync(service);
                    await _serviceRepository.CommitTransactionAsync();
                }
            }
            catch
            {
                await _serviceRepository.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Service> GetServiceEntityByIdAsync(int serviceId)
        {
            var service = await _serviceRepository.GetSingleAsync(s => s.ServiceID == serviceId);
            if (service == null)
                throw new KeyNotFoundException("Tjänsten hittades inte.");

            return service;
        }
    }
}
