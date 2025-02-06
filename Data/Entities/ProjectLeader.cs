using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ProjectLeader
    {
        public int ProjectLeaderID { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Department { get; set; }

        // Navigation
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }

}
