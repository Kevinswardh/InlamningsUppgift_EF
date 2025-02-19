using Business.DTOs;
using Business.Factories;
using Data.DatabaseRepository;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Provides implementations for customer-related operations.
    /// </summary>
    public class CustomerService : ICustomerService
    {

        private readonly IBaseRepository<Customer> _customerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class with a customer repository.
        /// </summary>
        /// <param name="customerRepository">The repository for customer data access.</param>
        public CustomerService(IBaseRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }


        /// <summary>
        /// Retrieves all customers from the repository asynchronously.
        /// </summary>
        /// <returns>A collection of <see cref="CustomerDTO"/> representing all customers.</returns>
        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => new CustomerDTO
            {
                CustomerID = c.CustomerID,
                CustomerName = c.CustomerName,
                OrganizationNumber = c.OrganizationNumber,
                Address = c.Address,
                Discount = c.Discount ?? 0 // Standardvärde om Discount är null
            }).ToList();
        }


        /// <summary>
        /// Creates a new customer in the repository asynchronously, using a transaction.
        /// </summary>
        /// <param name="customerDto">The customer data transfer object containing customer details.</param>
        /// <exception cref="Exception">Thrown when the transaction fails.</exception>
        public async Task CreateCustomerAsync(CustomerDTO customerDto)
        {
            await _customerRepository.BeginTransactionAsync();
            try
            {
                var customer = CustomerFactory.CreateCustomer(customerDto);
                await _customerRepository.AddAsync(customer);
                await _customerRepository.CommitTransactionAsync();
            }
            catch
            {
                await _customerRepository.RollbackTransactionAsync();
                throw;
            }
        }


        /// <summary>
        /// Deletes a customer by their ID from the repository asynchronously, using a transaction.
        /// </summary>
        /// <param name="customerId">The ID of the customer to delete.</param>
        /// <exception cref="Exception">Thrown if the customer cannot be found or the transaction fails.</exception>
        public async Task DeleteCustomerAsync(int customerId)
        {
            await _customerRepository.BeginTransactionAsync();
            try
            {
                var customer = await _customerRepository.GetSingleAsync(c => c.CustomerID == customerId);
                if (customer != null)
                {
                    await _customerRepository.DeleteAsync(customer);
                    await _customerRepository.CommitTransactionAsync();
                }
            }
            catch
            {
                await _customerRepository.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
