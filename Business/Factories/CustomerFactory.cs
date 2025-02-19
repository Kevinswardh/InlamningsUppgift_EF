namespace Business.Factories
{
    using Business.DTOs;
    using Data.Entities;

    /// <summary>
    /// Provides methods to create <see cref="Customer"/> entities from data transfer objects.
    /// </summary>
    public static class CustomerFactory
    {
        /// <summary>
        /// Creates a <see cref="Customer"/> entity from the given <see cref="CustomerDTO"/>.
        /// </summary>
        /// <param name="customerDto">The data transfer object containing customer information.</param>
        /// <returns>A new <see cref="Customer"/> entity with values from the provided DTO.</returns>
        public static Customer CreateCustomer(CustomerDTO customerDto)
        {
            return new Customer
            {
                CustomerName = customerDto.CustomerName,
                OrganizationNumber = customerDto.OrganizationNumber,
                Address = customerDto.Address,
                Discount = customerDto.Discount
            };
        }
    }
}
