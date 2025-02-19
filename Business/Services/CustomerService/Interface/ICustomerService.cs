using Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Defines methods for managing customer operations.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Retrieves all customers asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="CustomerDTO"/> representing all customers.</returns>
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();

        /// <summary>
        /// Creates a new customer asynchronously.
        /// </summary>
        /// <param name="customerDto">The customer data transfer object containing customer information.</param>
        Task CreateCustomerAsync(CustomerDTO customerDto);

        /// <summary>
        /// Deletes a customer asynchronously by their ID.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer to be deleted.</param>
        Task DeleteCustomerAsync(int customerId);
    }
}
