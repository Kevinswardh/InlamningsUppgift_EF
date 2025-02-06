using Data.Database;
using Data.Entities;

namespace Data.DatabaseRepository
{
    public class ProjectLeaderRepository : BaseRepository<ProjectLeader>, IProjectLeaderRepository
    {
        public ProjectLeaderRepository(AppDbContext context) : base(context) { }
    }
}
