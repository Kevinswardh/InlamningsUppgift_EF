using Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task CreateCustomerAsync(CustomerDTO customerDto);
        Task DeleteCustomerAsync(int customerId);
    }
}
