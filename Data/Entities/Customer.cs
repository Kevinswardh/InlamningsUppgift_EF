using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }

        [Required]
        public string CustomerName { get; set; } = null!;

        [Required]
        public string OrganizationNumber { get; set; } = null!;

        public string? Address { get; set; }
        public decimal? Discount { get; set; }

        // Navigation (Lazy Loading aktiveras med virtual)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
