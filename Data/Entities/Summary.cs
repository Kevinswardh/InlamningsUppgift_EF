using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SummaryID { get; set; }

        [Required]
        public int ProjectID { get; set; }

        public int? TotalHours { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public Project Project { get; set; } = null!;
    }
}
