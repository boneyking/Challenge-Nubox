using Integracion.Nubox.Api.Common.Entities;

namespace Integracion.Nubox.Api.Common.Persistence
{
    public interface IRepository<T, TId> where T: BaseEntity<TId>
    {
        Task<T?> GetByIdAsync(TId id);
        Task<IQueryable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        void Update(T entity);
    }
}
