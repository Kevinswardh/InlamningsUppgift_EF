using Data.Entities;

namespace Data.DatabaseRepository
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        // Här kan du lägga till specifika metoder för Customers om det behövs.
    }
}
