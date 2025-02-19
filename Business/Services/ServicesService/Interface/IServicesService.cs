using Business.DTOs;
using Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Defines methods for managing service operations.
    /// </summary>
    public interface IServiceService
    {
        /// <summary>
        /// Retrieves all services asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="ServiceDTO"/> representing all services.</returns>
        Task<IEnumerable<ServiceDTO>> GetAllServicesAsync();


        /// <summary>
        /// Creates a new service asynchronously.
        /// </summary>
        /// <param name="serviceDto">The data transfer object containing service information.</param>
        Task CreateServiceAsync(ServiceDTO serviceDto);


        /// <summary>
        /// Deletes a service asynchronously by its ID.
        /// </summary>
        /// <param name="serviceId">The unique identifier of the service to be deleted.</param>
        Task DeleteServiceAsync(int serviceId);


        /// <summary>
        /// Retrieves a <see cref="Service"/> entity by its ID asynchronously.
        /// </summary>
        /// <param name="serviceId">The unique identifier of the service.</param>
        /// <returns>A <see cref="Service"/> entity representing the requested service.</returns>
        Task<Service> GetServiceEntityByIdAsync(int serviceId);
    }
}
