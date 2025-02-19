using Data.Entities;

namespace Business.Factories
{
    /// <summary>
    /// Provides methods to create <see cref="ProjectLeader"/> entities.
    /// </summary>
    public static class ProjectLeaderFactory
    {
        /// <summary>
        /// Creates a <see cref="ProjectLeader"/> entity with the provided details.
        /// </summary>
        /// <param name="firstName">The first name of the project leader.</param>
        /// <param name="lastName">The last name of the project leader.</param>
        /// <param name="email">The email address of the project leader.</param>
        /// <param name="phone">The phone number of the project leader.</param>
        /// <param name="department">The department to which the project leader belongs.</param>
        /// <returns>A new <see cref="ProjectLeader"/> entity with the specified information.</returns>
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
