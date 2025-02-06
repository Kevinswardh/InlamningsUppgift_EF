namespace Business.Factories
{
    using Business.DTOs;
    using Data.Entities;

    public static class CustomerFactory
    {
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
