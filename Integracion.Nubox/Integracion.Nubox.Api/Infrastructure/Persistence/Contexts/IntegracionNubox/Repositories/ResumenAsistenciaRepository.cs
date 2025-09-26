using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public class ResumenAsistenciaRepository : Repository<ResumenAsistencia, Guid>, IResumenAsistenciaRepository
    {
        public ResumenAsistenciaRepository(IntegracionNuboxContext context) : base(context)
        {
        }

        public async Task<ResumenAsistencia?> GetByTrabajadorYPeriodoAsync(Guid trabajadorId, int año, int mes, int? semana = null, int? quincena = null)
        {
            return await Entities
                .Include(r => r.Trabajador)
                .FirstOrDefaultAsync(r =>
                    r.TrabajadorId == trabajadorId &&
                    r.Año == año &&
                    r.Mes == mes &&
                    r.Semana == semana &&
                    r.Quincena == quincena);
        }

        public async Task<List<ResumenAsistencia>> GetParaLiquidacionAsync(Guid companiaId, int año, int mes, int? semana = null, int? quincena = null)
        {
            return await Entities
                .Include(r => r.Trabajador)
                .Where(r =>
                    r.Trabajador.CompaniaId == companiaId &&
                    r.Año == año &&
                    r.Mes == mes &&
                    r.Semana == semana &&
                    r.Quincena == quincena &&
                    !r.EsProcesado)
                .ToListAsync();
        }

        public async Task<List<ResumenAsistencia>> GetSinProcesarAsync()
        {
            return await Entities
                .Include(r => r.Trabajador)
                .Where(r => !r.EsProcesado)
                .OrderBy(r => r.FechaCreacion)
                .ToListAsync();
        }

        public async Task<int> MarcarComoProcesadosAsync(List<Guid> ids, Guid liquidacionId)
        {
            return await Entities
                .Where(r => ids.Contains(r.Id))
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.EsProcesado, true)
                    .SetProperty(x => x.FechaProcesamiento, DateTime.UtcNow)
                    .SetProperty(x => x.LiquidacionId, liquidacionId));
        }
    }

}
