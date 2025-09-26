using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public interface IResumenAsistenciaService
    {
        Task GenerateResumenesAsync(Guid companiaId, DateTime fechaDesde, DateTime fechaHasta);
        Task<ResumenAsistencia> GenerateResumenForTrabajadorAsync(Guid trabajadorId, int año, int mes, int? semana = null, int? quincena = null);
        Task<List<ResumenAsistencia>> GetResumenesParaLiquidacionAsync(Guid companiaId, int año, int mes, int? semana = null, int? quincena = null);
    }
}
