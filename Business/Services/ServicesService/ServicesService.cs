using Business.DTOs;
using Data.DatabaseRepository;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Provides implementations for operations related to services.
    /// </summary>
    public class ServiceService : IServiceService
    {
        private readonly IBaseRepository<Service> _serviceRepository;


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceService"/> class with the specified repository.
        /// </summary>
        /// <param name="serviceRepository">The repository for service data access.</param>
        public ServiceService(IBaseRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }


        /// <summary>
        /// Retrieves all services from the repository asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="ServiceDTO"/> representing all services.</returns>
        public async Task<IEnumerable<ServiceDTO>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return services.Select(s => new ServiceDTO
            {
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName
            }).ToList();
        }


        /// <summary>
        /// Creates a new service in the repository asynchronously with transaction management.
        /// </summary>
        /// <param name="serviceDto">The data transfer object containing service details.</param>
        /// <exception cref="Exception">Thrown if the transaction fails during creation.</exception>
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


        /// <summary>
        /// Deletes a service by its ID asynchronously with transaction management.
        /// </summary>
        /// <param name="serviceId">The ID of the service to delete.</param>
        /// <exception cref="Exception">Thrown if the transaction fails or the service is not found.</exception>
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


        /// <summary>
        /// Retrieves a <see cref="Service"/> entity by its ID asynchronously.
        /// </summary>
        /// <param name="serviceId">The unique identifier of the service.</param>
        /// <returns>A <see cref="Service"/> entity representing the requested service.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the service is not found.</exception>
        public async Task<Service> GetServiceEntityByIdAsync(int serviceId)
        {
            var service = await _serviceRepository.GetSingleAsync(s => s.ServiceID == serviceId);
            if (service == null)
                throw new KeyNotFoundException("Tjänsten hittades inte.");

            return service;
        }
    }
}
