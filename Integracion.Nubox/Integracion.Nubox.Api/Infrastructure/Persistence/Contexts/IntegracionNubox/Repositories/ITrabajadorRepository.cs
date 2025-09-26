using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public interface ITrabajadorRepository : IRepository<Trabajador, Guid>
    {
        Task<Trabajador?> GetByDniAndCompaniaAsync(string dni, Guid companiaId);
        Task<List<Trabajador>> GetByCompaniaAsync(Guid companiaId, bool soloActivos = true);
        Task<Trabajador?> GetByIdExternoPartnerAsync(string idExternoPartner);
        Task<List<Trabajador>> GetSinSincronizarAsync(DateTime? ultimaSincronizacion = null);
    }

    public interface IResumenAsistenciaRepository : IRepository<ResumenAsistencia, Guid>
    {
        Task<ResumenAsistencia?> GetByTrabajadorYPeriodoAsync(Guid trabajadorId, int año, int mes, int? semana = null, int? quincena = null);
        Task<List<ResumenAsistencia>> GetParaLiquidacionAsync(Guid companiaId, int año, int mes, int? semana = null, int? quincena = null);
        Task<List<ResumenAsistencia>> GetSinProcesarAsync();
        Task<int> MarcarComoProcesadosAsync(List<Guid> ids, Guid liquidacionId);
    }

}
