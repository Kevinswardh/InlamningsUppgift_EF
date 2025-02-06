namespace Business.DTOs
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string OrganizationNumber { get; set; }
        public string? Address { get; set; }
        public decimal? Discount { get; set; } 
    }

}
