using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public class ResumenAsistenciaService : IResumenAsistenciaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResumenAsistenciaService> _logger;

        public ResumenAsistenciaService(IUnitOfWork unitOfWork, ILogger<ResumenAsistenciaService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task GenerateResumenesAsync(Guid companiaId, DateTime fechaDesde, DateTime fechaHasta)
        {
            _logger.LogInformation("Generando resúmenes de asistencia para compañía {CompaniaId} desde {FechaDesde} hasta {FechaHasta}",
                companiaId, fechaDesde, fechaHasta);

            var trabajadores = await _unitOfWork.Trabajadores.GetByCompaniaAsync(companiaId, soloActivos: true);

            var mesesProcesar = GetMesesEnRango(fechaDesde, fechaHasta);

            foreach (var trabajador in trabajadores)
            {
                foreach (var (año, mes) in mesesProcesar)
                {
                    try
                    {
                        await GenerateResumenForTrabajadorAsync(trabajador.Id, año, mes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generando resumen para trabajador {TrabajadorId} en período {Año}-{Mes}",
                            trabajador.Id, año, mes);
                    }
                }
            }

            _logger.LogInformation("Resúmenes generados exitosamente para {TrabajadoresCount} trabajadores",
                trabajadores.Count);
        }

        public async Task<ResumenAsistencia> GenerateResumenForTrabajadorAsync(Guid trabajadorId, int año, int mes, int? semana = null, int? quincena = null)
        {
            var resumenExistente = await _unitOfWork.ResumenesAsistencia
                .GetByTrabajadorYPeriodoAsync(trabajadorId, año, mes, semana, quincena);

            if (resumenExistente is not null)
            {
                await UpdateResumenAsync(resumenExistente);
                return resumenExistente;
            }

            var fechaDesde = new DateTime(año, mes, 1);
            var fechaHasta = fechaDesde.AddMonths(1).AddDays(-1);

            if (semana.HasValue)
            {
                var primerDiaSemana = GetFirstDayOfWeek(año, semana.Value);
                fechaDesde = primerDiaSemana;
                fechaHasta = primerDiaSemana.AddDays(6);
            }
            else if (quincena.HasValue)
            {
                if (quincena.Value == 1)
                {
                    fechaDesde = new DateTime(año, mes, 1);
                    fechaHasta = new DateTime(año, mes, 15);
                }
                else
                {
                    fechaDesde = new DateTime(año, mes, 16);
                    fechaHasta = fechaDesde.AddMonths(1).AddDays(-1);
                }
            }

            var registros = await _unitOfWork.RegistrosAsistencia
                .GetByTrabajadorYPeriodoAsync(trabajadorId, fechaDesde, fechaHasta);

            var resumen = new ResumenAsistencia
            {
                Id = Guid.NewGuid(),
                TrabajadorId = trabajadorId,
                Año = año,
                Mes = mes,
                Semana = semana,
                Quincena = quincena,
                TotalHorasRegulares = registros.Sum(r => r.HorasRegulares),
                TotalHorasExtras = registros.Sum(r => r.HorasExtras),
                DiasAsistencia = registros.Count(r => r.Tipo == TipoAsistencia.Presente),
                DiasInasistencia = registros.Count(r => r.Tipo == TipoAsistencia.Ausente),
                DiasLicencia = registros.Count(r => r.Tipo == TipoAsistencia.LicenciaMedica),
                DiasVacaciones = registros.Count(r => r.Tipo == TipoAsistencia.Vacaciones),
                DiasTardanza = registros.Count(r => r.HoraEntrada.HasValue && r.HoraEntrada.Value > TimeSpan.FromHours(8)), // Asumiendo 8 AM como hora de entrada
                EsProcesado = false,
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            };

            await _unitOfWork.ResumenesAsistencia.AddAsync(resumen);
            await _unitOfWork.SaveChangesAsync();

            return resumen;
        }

        public async Task<List<ResumenAsistencia>> GetResumenesParaLiquidacionAsync(Guid companiaId, int año,
            int mes, int? semana = null, int? quincena = null)
                => await _unitOfWork.ResumenesAsistencia
            .GetParaLiquidacionAsync(companiaId, año, mes, semana, quincena);

        private async Task UpdateResumenAsync(ResumenAsistencia resumen)
        {
            var fechaDesde = new DateTime(resumen.Año, resumen.Mes, 1);
            var fechaHasta = fechaDesde.AddMonths(1).AddDays(-1);

            var registros = await _unitOfWork.RegistrosAsistencia
                .GetByTrabajadorYPeriodoAsync(resumen.TrabajadorId, fechaDesde, fechaHasta);

            resumen.TotalHorasRegulares = registros.Sum(r => r.HorasRegulares);
            resumen.TotalHorasExtras = registros.Sum(r => r.HorasExtras);
            resumen.DiasAsistencia = registros.Count(r => r.Tipo == TipoAsistencia.Presente);
            resumen.DiasInasistencia = registros.Count(r => r.Tipo == TipoAsistencia.Ausente);
            resumen.DiasLicencia = registros.Count(r => r.Tipo == TipoAsistencia.LicenciaMedica);
            resumen.DiasVacaciones = registros.Count(r => r.Tipo == TipoAsistencia.Vacaciones);
            resumen.FechaActualizacion = DateTime.UtcNow;

            _unitOfWork.ResumenesAsistencia.Update(resumen);
            await _unitOfWork.SaveChangesAsync();
        }

        private List<(int año, int mes)> GetMesesEnRango(DateTime fechaDesde, DateTime fechaHasta)
        {
            var meses = new List<(int, int)>();
            var fecha = new DateTime(fechaDesde.Year, fechaDesde.Month, 1);
            var fechaFin = new DateTime(fechaHasta.Year, fechaHasta.Month, 1);

            while (fecha <= fechaFin)
            {
                meses.Add((fecha.Year, fecha.Month));
                fecha = fecha.AddMonths(1);
            }

            return meses;
        }

        private DateTime GetFirstDayOfWeek(int year, int week)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            return firstMonday.AddDays((week - 1) * 7);
        }
    }
}
