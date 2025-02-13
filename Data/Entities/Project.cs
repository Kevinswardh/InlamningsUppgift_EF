using System;
using System.Collections.Generic;
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

        // Navigation (Lazy Loading aktiveras med virtual)
        public virtual ProjectLeader ProjectLeader { get; set; } = null!; // 🔹 Lazy Loading fungerar nu
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); // 🔹 Lazy Loading fungerar nu
        public virtual Summary Summary { get; set; } // 🔹 Lazy Loading fungerar nu
    }
}
