using Data.Entities;

namespace Business.Factories
{
    public static class ProjectLeaderFactory
    {
        public static ProjectLeader Create(string firstName, string lastName, string email, string phone, string department)
        {
            return new ProjectLeader
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Department = department
            };
        }
    }
}
