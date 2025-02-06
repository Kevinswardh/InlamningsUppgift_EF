using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectID { get; set; }

        [Required]
        public string ProjectNumber { get; set; } = null!;

        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }

        // Foreign Key
        public int ProjectLeaderID { get; set; }

        // Navigation
        public ProjectLeader ProjectLeader { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public Summary? Summary { get; set; }
    }

}
