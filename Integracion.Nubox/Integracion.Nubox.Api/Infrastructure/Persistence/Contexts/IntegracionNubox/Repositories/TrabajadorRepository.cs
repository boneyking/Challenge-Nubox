using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public class TrabajadorRepository : Repository<Trabajador, Guid>, ITrabajadorRepository
    {
        public TrabajadorRepository(IntegracionNuboxContext context) : base(context)
        {
        }

        public async Task<Trabajador?> GetByDniAndCompaniaAsync(string dni, Guid companiaId)
        {
            return await Entities
                .Include(t => t.Compania)
                .FirstOrDefaultAsync(t => t.Dni == dni && t.CompaniaId == companiaId);
        }

        public async Task<List<Trabajador>> GetByCompaniaAsync(Guid companiaId, bool soloActivos = true)
        {
            var query = Entities
                .Include(t => t.Compania)
                .Where(t => t.CompaniaId == companiaId);

            if (soloActivos)
                query = query.Where(t => t.EsActivo);

            return await query.ToListAsync();
        }

        public async Task<Trabajador?> GetByIdExternoPartnerAsync(string idExternoPartner)
        {
            return await Entities
                .Include(t => t.Compania)
                .FirstOrDefaultAsync(t => t.IdExternoPartner == idExternoPartner);
        }

        public async Task<List<Trabajador>> GetSinSincronizarAsync(DateTime? ultimaSincronizacion = null)
        {
            var fecha = ultimaSincronizacion ?? DateTime.UtcNow.AddDays(-1);

            return await Entities
                .Include(t => t.Compania)
                .Where(t => t.EsActivo && (!t.UltimaSincronizacion.HasValue || t.UltimaSincronizacion < fecha))
                .ToListAsync();
        }
    }
}
