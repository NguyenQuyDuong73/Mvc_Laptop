using MvcLaptop.Models;
using Microsoft.EntityFrameworkCore.Storage;
using MvcLaptop.Data;

namespace Mvclaptop.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly MvcLaptopContext _dbContext;
        private IDbContextTransaction? _transaction;
        public IRepository<User> _userRepository { get; }
        public IRepository<Role> _roleRepository { get; }
        public IRepository<Permission> _permissionRepository { get; }
        public IRepository<Function> _functionRepository { get; }
        public IRepository<Command> _commandRepository { get; }

        private bool _disposed = false; // To track whether the object has been disposed

        /// <summary>
        /// Constructor initializes the ApplicationDbContext and repositories.
        /// </summary>
        /// <param name="dbContext">The database context to be used.</param>
        public UnitOfWork(MvcLaptopContext dbContext)
        {
            _dbContext = dbContext;
            _userRepository = new Repository<User>(_dbContext);
            _roleRepository = new Repository<Role>(_dbContext);
            _permissionRepository = new Repository<Permission>(_dbContext);
            _functionRepository = new Repository<Function>(_dbContext);
            _commandRepository = new Repository<Command>(_dbContext);
        }

        /// <summary>
        /// Starts a new database transaction.
        /// </summary>
        public void CreateTransaction()
        {
            _transaction = _dbContext.Database.BeginTransaction();
        }

        /// <summary>
        /// Starts a new database transaction asynchronously.
        /// </summary>
        public async Task CreateTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction. Saves all changes and applies the transaction.
        /// </summary>
        public void Commit()
        {
            try
            {
                SaveChanges();
                _transaction?.Commit();
            }
            catch
            {
                // If something goes wrong, roll back the transaction
                Rollback();
                throw;
            }
            finally
            {
                // Ensure transaction resources are released
                _transaction?.Dispose();
            }
        }

        /// <summary>
        /// Commits the current transaction asynchronously. Saves all changes and applies the transaction.
        /// </summary>
        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                // If something goes wrong, roll back the transaction
                await RollbackAsync();
                throw;
            }
            finally
            {
                // Ensure transaction resources are released
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
        }

        /// <summary>
        /// Rolls back the current transaction asynchronously.
        /// </summary>
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        /// Saves changes to the database asynchronously.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the UnitOfWork object, releasing all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">True if called from Dispose method, false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources like DbContext and transaction
                    _dbContext.Dispose();
                    _transaction?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
