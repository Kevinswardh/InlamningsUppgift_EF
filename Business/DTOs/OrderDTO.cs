namespace Business.DTOs
{
    public class OrderDTO
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int ProjectID { get; set; }
        public decimal Hours { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Hours * Price;

        // Relationer
        public CustomerDTO Customer { get; set; }
        public ServiceDTO Service { get; set; }
    }
}
