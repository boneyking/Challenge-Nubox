using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public class TransaccionSincronizacionRepository : Repository<TransaccionSincronizacion, Guid>, ITransaccionSincronizacionRepository
    {
        public TransaccionSincronizacionRepository(IntegracionNuboxContext context) : base(context)
        {
        }

        public async Task<TransaccionSincronizacion?> GetByTransactionIdAsync(string transactionId)
        {
            return await Entities
                .Include(t => t.Compania)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<List<TransaccionSincronizacion>> GetByCompaniaAsync(Guid companiaId, int take = 100)
        {
            return await Entities
                .Include(t => t.Compania)
                .Where(t => t.CompaniaId == companiaId)
                .OrderByDescending(t => t.FechaInicio)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<TransaccionSincronizacion>> GetTransaccionesPendientesAsync()
        {
            return await Entities
                .Include(t => t.Compania)
                .Where(t => t.Estado == EstadoProcesamiento.Procesando)
                .Where(t => t.FechaInicio < DateTime.UtcNow.AddHours(-1))
                .ToListAsync();
        }

        public async Task<List<TransaccionSincronizacion>> GetTransaccionesConErrorAsync()
        {
            return await Entities
                .Include(t => t.Compania)
                .Where(t => t.Estado == EstadoProcesamiento.Error)
                .OrderByDescending(t => t.FechaInicio)
                .Take(50)
                .ToListAsync();
        }

    }
}
