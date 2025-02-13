using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceID { get; set; }

        [Required]
        public string ServiceName { get; set; } = null!;

        // Navigation (Lazy Loading aktiveras med virtual)
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
