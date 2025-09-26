using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public interface IConfiguracionPartnerRepository : IRepository<ConfiguracionPartner, Guid>
    {
        Task<ConfiguracionPartner?> GetByCompaniaAsync(Guid companiaId);
        Task<List<ConfiguracionPartner>> GetConfiguracionesActivasAsync();
        Task<List<ConfiguracionPartner>> GetParaSincronizarAsync();
    }

}
