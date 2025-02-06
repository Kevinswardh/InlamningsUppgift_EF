using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Order
    {
        // Composite Key: CustomerID + ServiceID
        [Key]
        [Column(Order = 0)]
        public int CustomerID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ServiceID { get; set; }

        // Other Fields
        [Required]
        public int ProjectID { get; set; }

        [Required]
        public decimal Hours { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Navigation
        public Customer Customer { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
