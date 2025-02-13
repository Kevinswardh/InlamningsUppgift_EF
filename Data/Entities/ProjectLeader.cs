using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public class ProjectLeader
    {
        public int ProjectLeaderID { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Department { get; set; }

        // Navigation (Lazy Loading aktiveras med virtual)
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
