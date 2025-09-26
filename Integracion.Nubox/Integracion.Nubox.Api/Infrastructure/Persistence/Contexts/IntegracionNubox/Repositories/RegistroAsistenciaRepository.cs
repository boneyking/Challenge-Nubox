using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.EntityFrameworkCore;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories
{
    public class RegistroAsistenciaRepository : Repository<RegistroAsistencia, Guid>, IRegistroAsistenciaRepository
    {
        public RegistroAsistenciaRepository(IntegracionNuboxContext context) : base(context)
        {
        }

        public async Task<List<RegistroAsistencia>> GetByTrabajadorYPeriodoAsync(Guid trabajadorId, DateTime desde, DateTime hasta)
        {
            return await Entities
                .Include(r => r.Trabajador)
                .Where(r => r.TrabajadorId == trabajadorId && r.Fecha >= desde && r.Fecha <= hasta)
                .OrderBy(r => r.Fecha)
                .ToListAsync();
        }

        public async Task<List<RegistroAsistencia>> GetByCompaniaYPeriodoAsync(Guid companiaId, DateTime desde, DateTime hasta)
        {
            return await Entities
                .Include(r => r.Trabajador)
                .Where(r => r.Trabajador.CompaniaId == companiaId && r.Fecha >= desde && r.Fecha <= hasta)
                .OrderBy(r => r.Fecha)
                .ThenBy(r => r.Trabajador.Apellidos)
                .ToListAsync();
        }

        public async Task<List<RegistroAsistencia>> GetPendientesProcesarAsync()
        {
            return await Entities
                .Include(r => r.Trabajador)
                .Where(r => r.Estado == EstadoProcesamiento.Pendiente)
                .OrderBy(r => r.FechaRecepcion)
                .ToListAsync();
        }

        public async Task<bool> ExisteRegistroAsync(Guid trabajadorId, DateTime fecha)
        {
            return await Entities
                .AnyAsync(r => r.TrabajadorId == trabajadorId && r.Fecha.Date == fecha.Date);
        }

        public async Task<int> BulkUpdateEstadoAsync(List<Guid> ids, EstadoProcesamiento estado)
        {
            return await Entities
                .Where(r => ids.Contains(r.Id))
                .ExecuteUpdateAsync(r => r
                    .SetProperty(x => x.Estado, estado)
                    .SetProperty(x => x.FechaActualizacion, DateTime.UtcNow));
        }
    }
}
