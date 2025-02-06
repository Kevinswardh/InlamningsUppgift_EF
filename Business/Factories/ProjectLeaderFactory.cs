using Data.Entities;

namespace Business.Factories
{
    public static class ProjectLeaderFactory
    {
        public static ProjectLeader Create(string name, string email, string phone, string department)
        {
            return new ProjectLeader
            {
                Name = name,
                Email = email,
                Phone = phone,
                Department = department
            };
        }
    }
}
