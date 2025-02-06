using Data.Database;
using Data.Entities;

namespace Data.DatabaseRepository
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        public ServiceRepository(AppDbContext context) : base(context) { }
    }
}
