using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public class ConfiguracionPartnerRepository : Repository<ConfiguracionPartner, Guid>, IConfiguracionPartnerRepository
    {
        public ConfiguracionPartnerRepository(IntegracionNuboxContext context) : base(context)
        {
        }

        public async Task<ConfiguracionPartner?> GetByCompaniaAsync(Guid companiaId)
        {
            return await Entities
                .Include(c => c.Compania)
                .FirstOrDefaultAsync(c => c.CompaniaId == companiaId);
        }

        public async Task<List<ConfiguracionPartner>> GetConfiguracionesActivasAsync()
        {
            return await Entities
                .Include(c => c.Compania)
                .Where(c => c.EsActivo)
                .ToListAsync();
        }

        public async Task<List<ConfiguracionPartner>> GetParaSincronizarAsync()
        {
            var ahora = DateTime.UtcNow;

            return await Entities
                .Include(c => c.Compania)
                .Where(c => c.EsActivo && c.SincronizacionAutomatica)
                .Where(c => !c.ProximaSincronizacion.HasValue || c.ProximaSincronizacion <= ahora)
                .ToListAsync();
        }
    }

}
