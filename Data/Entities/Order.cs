using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Order
    {
        // ✅ **Sammansatt Primärnyckel: ProjectID + CustomerID + ServiceID**
        [Key]
        [Column(Order = 0)]
        public int ProjectID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CustomerID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int ServiceID { get; set; }

        [Required]
        public decimal Hours { get; set; }

        [Required]
        public decimal Price { get; set; }

        // 🔹 **Navigationsproperties med Lazy Loading**
        public virtual Project Project { get; set; } = null!;   // 🔹 Lazy Loading fungerar nu
        public virtual Customer Customer { get; set; } = null!; // 🔹 Lazy Loading fungerar nu
        public virtual Service Service { get; set; } = null!;   // 🔹 Lazy Loading fungerar nu
    }
}
