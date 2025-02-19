using Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.DatabaseRepository
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IBaseRepository{TEntity}"/> interface for standard CRUD operations and transaction management.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private IDbContextTransaction _transaction; // Privat transaktionshantering


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{TEntity}"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        /// <exception cref="ArgumentNullException">Thrown when the context is <c>null</c>.</exception>
        public BaseRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }


        /// <summary>
        /// Begins a new transaction if none is currently active.
        /// </summary>
        public virtual async Task BeginTransactionAsync()
        {
            if (_context.Database.CurrentTransaction == null) // ✅ Kolla om transaktion redan finns
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }


        /// <summary>
        /// Commits the active transaction and disposes of it.
        /// </summary>
        public virtual async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }


        /// <summary>
        /// Rolls back the active transaction and disposes of it.
        /// </summary>
        public virtual async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }


        // Virtuella metoder som kan skrivas över i subklasser


        /// <summary>
        /// Retrieves all entities of type <typeparamref name="TEntity"/> from the database asynchronously.
        /// </summary>
        /// <returns>A collection of all entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }


        /// <summary>
        /// Retrieves the first entity matching the provided predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The condition to filter entities.</param>
        /// <returns>The matching entity, or <c>null</c> if no match is found.</returns>
        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }



        // Standard CRUD-operationer


        /// <summary>
        /// Adds a new entity to the database asynchronously and saves the changes.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Finds all entities that match the given predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The condition to filter entities.</param>
        /// <returns>A collection of matching entities.</returns>
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }


        /// <summary>
        /// Updates an existing entity in the database and saves the changes asynchronously.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Removes an entity from the database and saves the changes asynchronously.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
