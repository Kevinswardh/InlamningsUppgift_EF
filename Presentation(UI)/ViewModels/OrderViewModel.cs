namespace Presentation_UI_.ViewModels
{
    public class OrderViewModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int ProjectID { get; set; }
        public decimal Hours { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Hours * Price;

        // Navigation Properties
        public CustomerViewModel Customer { get; set; }
        public ServiceViewModel Service { get; set; }
    }
}
