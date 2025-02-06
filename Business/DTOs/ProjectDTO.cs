namespace Business.DTOs
{
    public class ProjectDTO
    {
        public int ProjectID { get; set; }
        public string ProjectNumber { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int ProjectLeaderID { get; set; }

        // Relationer
        public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
        public SummaryDTO Summary { get; set; }
    }
}
