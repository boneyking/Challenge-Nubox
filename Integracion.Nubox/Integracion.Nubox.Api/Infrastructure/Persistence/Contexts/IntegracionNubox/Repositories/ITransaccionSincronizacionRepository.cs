using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public interface ITransaccionSincronizacionRepository : IRepository<TransaccionSincronizacion, Guid>
    {
        Task<TransaccionSincronizacion?> GetByTransactionIdAsync(string transactionId);
        Task<List<TransaccionSincronizacion>> GetByCompaniaAsync(Guid companiaId, int take = 100);
        Task<List<TransaccionSincronizacion>> GetTransaccionesPendientesAsync();
        Task<List<TransaccionSincronizacion>> GetTransaccionesConErrorAsync();
    }
}
