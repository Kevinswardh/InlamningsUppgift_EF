using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SummaryID { get; set; }

        [Required]
        public int ProjectID { get; set; }

        public decimal? TotalHours { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Notes { get; set; }

        // Navigation (Lazy Loading aktiveras med virtual)
        public virtual Project Project { get; set; } = null!;
    }
}
