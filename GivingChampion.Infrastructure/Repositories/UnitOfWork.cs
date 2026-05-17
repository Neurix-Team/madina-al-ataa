using GivingChampion.Persistence.Contexts;
using GivingChampion.Domain.Entities.Base;
using GivingChampion.Persistance.Interfaces;

namespace GivingChampion.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var entityType = typeof(TEntity);

            if (!_repositories.TryGetValue(entityType, out var repository))
            {
                repository = new GenericRepository<TEntity>(_context);
                _repositories.Add(entityType, repository);
            }

            return (IGenericRepository<TEntity>)repository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                await _context.DisposeAsync();
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}
