using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Navigation
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
