using System.Linq.Expressions;
using MvcLaptop.Models;
using Microsoft.EntityFrameworkCore;
using MvcLaptop.Data;

namespace Mvclaptop.Repositories
{
    /// <summary>
    /// Interface for a generic repository pattern.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        T? GetById(int id);

        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        T? GetById(string id);

        /// <summary>
        /// Asynchronously gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
        Task<T?> GetByIdAsync(string id);

        /// <summary>
        /// Gets all entities, optionally including related entities.
        /// </summary>
        /// <param name="includeProperties">An array of expressions representing the navigation properties to include. If no properties are specified, only the main entities will be retrieved.</param>
        /// <returns>An enumerable of all entities, potentially including related entities specified by the include properties.</returns>
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[]? includeProperties);

        /// <summary>
        /// Asynchronously gets all entities, optionally including related entities.
        /// </summary>
        /// <param name="includeProperties">An array of expressions representing the navigation properties to include. If no properties are specified, only the main entities will be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of all entities, potentially including related entities specified by the include properties.</returns>
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? includeProperties);

        /// <summary>
        /// Finds an entity based on a predicate and includes related entities if specified.
        /// </summary>
        /// <param name="predicate">The expression to filter entities.</param>
        /// <param name="includeProperties">The properties to include in the query.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        T? Find(Expression<Func<T, bool>> predicate, params string[]? includeProperties);

        /// <summary>
        /// Asynchronously finds an entity based on a predicate and includes related entities if specified.
        /// </summary>
        /// <param name="predicate">The expression to filter entities.</param>
        /// <param name="includeProperties">The properties to include in the query.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, params string[]? includeProperties);

        /// <summary>
        /// Gets the total count of entities.
        /// </summary>
        /// <returns>The total count of entities.</returns>
        int Count();

        /// <summary>
        /// Gets the total count of entities that satisfy a condition.
        /// </summary>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>The total count of entities that satisfy the condition.</returns>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously gets the total count of entities.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the total count of entities.</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Asynchronously gets the total count of entities that satisfy a condition.
        /// </summary>
        /// <param name="predicate">A function to test each entity for a condition.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the total count of entities that satisfy the condition.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        /// <summary>
        /// Asynchronously adds a new entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds multiple new entities.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Asynchronously adds multiple new entities.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Removes an existing entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Removes multiple existing entities.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Checks if an entity exists synchronously based on its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to check.</param>
        /// <returns>True if the entity exists; otherwise, false.</returns>
        bool Exists(int id);

        /// <summary>
        /// Asynchronously checks if an entity exists based on its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to check.</param>
        /// <returns>A task representing the asynchronous operation. The task result is true if the entity exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Checks if an entity exists synchronously based on its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to check.</param>
        /// <returns>True if the entity exists; otherwise, false.</returns>
        bool Exists(string id);

        /// <summary>
        /// Asynchronously checks if an entity exists based on its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to check.</param>
        /// <returns>A task representing the asynchronous operation. The task result is true if the entity exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Executes a raw SQL query to fetch entities.
        /// </summary>
        /// <param name="sql">The raw SQL query.</param>
        /// <param name="allowTracking">Specifies whether to enable change tracking.</param>
        /// <returns>An enumerable of entities that match the SQL query.</returns>
        IEnumerable<T> FromSqlQuery(string sql, bool allowTracking = true);

        /// <summary>
        /// Asynchronously executes a raw SQL query to fetch entities.
        /// </summary>
        /// <param name="sql">The raw SQL query.</param>
        /// <param name="allowTracking">Specifies whether to enable change tracking.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of entities that match the SQL query.</returns>
        Task<IEnumerable<T>> FromSqlQueryAsync(string sql, bool allowTracking = true);

        /// <summary>
        /// Retrieves multiple entities based on a predicate.
        /// </summary>
        /// <param name="predicate">The expression used to filter entities.</param>
        /// <param name="queryOperation">Optional. A function to apply additional operations on the query.</param>
        /// <param name="allowTracking">Optional. Indicates whether to enable change tracking.</param>
        /// <returns>An enumerable of entities that match the predicate.</returns>
        IEnumerable<T> GetMany(Expression<Func<T, bool>>? predicate = null,
                               Func<IQueryable<T>, IQueryable<T>>? queryOperation = null,
                               bool allowTracking = true);

        /// <summary>
        /// Asynchronously retrieves multiple entities based on a predicate.
        /// </summary>
        /// <param name="predicate">The expression used to filter entities.</param>
        /// <param name="queryOperation">Optional. A function to apply additional operations on the query.</param>
        /// <param name="allowTracking">Optional. Indicates whether to enable change tracking.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an enumerable of entities that match the predicate.</returns>
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>>? predicate = null,
                                          Func<IQueryable<T>, IQueryable<T>>? queryOperation = null,
                                          bool allowTracking = true);

        /// <summary>
        /// Executes a LINQ query against the repository.
        /// </summary>
        /// <param name="query">The LINQ query to execute.</param>
        /// <returns>An IQueryable of entities that match the query.</returns>
        IQueryable<T> ExecuteQuery(Expression<Func<T, bool>> query);

        /// <summary>
        /// Asynchronously executes a LINQ query against the repository.
        /// </summary>
        /// <param name="query">The LINQ query to execute.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an IQueryable of entities that match the query.</returns>
        Task<IQueryable<T>> ExecuteQueryAsync(Expression<Func<T, bool>> query);

    }

    /// <summary>
    /// Generic implementation of IRepository interface.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> _entities;

        public Repository(MvcLaptopContext dbContext)
        {
            _entities = dbContext.Set<T>();
        }

        public T? GetById(int id)
        {
            return _entities.Find(id);
        }

        public T? GetById(string id)
        {
            return _entities.Find(id);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _entities.FindAsync(id);
        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[]? includeProperties)
        {
            IQueryable<T> query = _entities;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[]? includeProperties)
        {
            IQueryable<T> query = _entities;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.ToListAsync();
        }
        public T? Find(Expression<Func<T, bool>> predicate, params string[]? includeProperties)
        {
            IQueryable<T> query = _entities;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault(predicate);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, params string[]? includeProperties)
        {
            IQueryable<T> query = _entities;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public int Count()
        {
            return _entities.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _entities.Count(predicate);
        }

        public async Task<int> CountAsync()
        {
            return await _entities.CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entities.CountAsync(predicate);
        }

        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _entities.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _entities.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public void Remove(T entity)
        {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _entities.RemoveRange(entities);
        }

        public bool Exists(int id)
        {
            return _entities.Find(id) != null; // Assuming Id is the primary key
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _entities.FindAsync(id) != null; // Assuming Id is the primary key
        }

        public bool Exists(string id)
        {
            return _entities.Find(id) != null; // Assuming Id is the primary key
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _entities.FindAsync(id) != null; // Assuming Id is the primary key
        }

        public IEnumerable<T> FromSqlQuery(string sql, bool allowTracking = true)
        {
            return _entities.FromSqlRaw(sql).ToList();
        }

        public async Task<IEnumerable<T>> FromSqlQueryAsync(string sql, bool allowTracking = true)
        {
            return await _entities.FromSqlRaw(sql).ToListAsync();
        }

        public IEnumerable<T> GetMany(Expression<Func<T, bool>>? predicate = null,
                                       Func<IQueryable<T>, IQueryable<T>>? queryOperation = null,
                                       bool allowTracking = true)
        {
            var query = _entities.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyTracking(query, allowTracking);

            if (queryOperation != null)
            {
                query = queryOperation(query);
            }

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>>? predicate = null,
                                                       Func<IQueryable<T>, IQueryable<T>>? queryOperation = null,
                                                       bool allowTracking = true)
        {
            var query = _entities.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = ApplyTracking(query, allowTracking);

            if (queryOperation != null)
            {
                query = queryOperation(query);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Applies tracking based on the allowTracking parameter.
        /// </summary>
        /// <param name="query">The query to apply tracking to.</param>
        /// <param name="allowTracking">Indicates whether to enable change tracking.</param>
        /// <returns>The query with tracking applied.</returns>
        private IQueryable<T> ApplyTracking(IQueryable<T> query, bool allowTracking)
        {
            return allowTracking ? query.AsTracking() : query.AsNoTracking();
        }

        public IQueryable<T> ExecuteQuery(Expression<Func<T, bool>> query)
        {
            return _entities.Where(query);
        }

        public async Task<IQueryable<T>> ExecuteQueryAsync(Expression<Func<T, bool>> query)
        {
            return await Task.FromResult(_entities.Where(query));
        }
    }
}
