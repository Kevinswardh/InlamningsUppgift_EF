using Business.DTOs;
using Business.Factories;
using Data.DatabaseRepository;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IBaseRepository<Customer> _customerRepository;

        public CustomerService(IBaseRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

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

        public async Task CreateCustomerAsync(CustomerDTO customerDto)
        {
            // Använd CustomerFactory för att skapa en Customer-instans
            var customer = CustomerFactory.CreateCustomer(customerDto);
            await _customerRepository.AddAsync(customer);
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _customerRepository.GetSingleAsync(c => c.CustomerID == customerId);
            if (customer != null)
            {
                await _customerRepository.DeleteAsync(customer);
            }
        }
    }
}
