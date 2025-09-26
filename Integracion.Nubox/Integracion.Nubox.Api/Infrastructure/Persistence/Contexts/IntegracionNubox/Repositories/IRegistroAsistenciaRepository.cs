using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public interface IRegistroAsistenciaRepository : IRepository<RegistroAsistencia, Guid>
    {
        Task<List<RegistroAsistencia>> GetByTrabajadorYPeriodoAsync(Guid trabajadorId, DateTime desde, DateTime hasta);
        Task<List<RegistroAsistencia>> GetByCompaniaYPeriodoAsync(Guid companiaId, DateTime desde, DateTime hasta);
        Task<List<RegistroAsistencia>> GetPendientesProcesarAsync();
        Task<bool> ExisteRegistroAsync(Guid trabajadorId, DateTime fecha);
        Task<int> BulkUpdateEstadoAsync(List<Guid> ids, EstadoProcesamiento estado);
    }
}
