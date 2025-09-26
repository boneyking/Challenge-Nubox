using Integracion.Nubox.Api.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Integracion.Nubox.Api.Common.Persistence
{
    public abstract class Repository<T, TId> : IRepository<T, TId>
        where T : BaseEntity<TId>
        where TId : IEquatable<TId>
    {
        private readonly DbContext _context;
        protected DbSet<T> Entities => _context.Set<T>();
        public Repository(DbContext context) => _context = context;

        public async Task<T> AddAsync(T entity)
        {
            EntityEntry<T> entry = await Entities.AddAsync(entity);
            return entry.Entity;
        }

        public async Task<IQueryable<T>> GetAllAsync() => await Task.FromResult(Entities.AsQueryable());
        public async Task<T?> GetByIdAsync(TId id) => await Entities.FirstOrDefaultAsync(x => x.Id.Equals(id));
        public void Update(T entity) => Entities.Update(entity);
    }
}
