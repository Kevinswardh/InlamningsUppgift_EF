using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    /// <summary>
    /// Defines the basic CRUD operations and transaction management for data repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Adds a new entity to the repository asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Retrieves a single entity that matches the specified predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The expression to filter entities.</param>
        /// <returns>The matching entity, or <c>null</c> if not found.</returns>
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Retrieves all entities in the repository asynchronously.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Finds all entities that match the specified predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The expression to filter entities.</param>
        /// <returns>A collection of matching entities.</returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Updates an existing entity in the repository asynchronously.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Deletes an entity from the repository asynchronously.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        Task DeleteAsync(TEntity entity);



        //Transaktion managment


        /// <summary>
        /// Begins a database transaction asynchronously.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction asynchronously.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current database transaction asynchronously.
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
