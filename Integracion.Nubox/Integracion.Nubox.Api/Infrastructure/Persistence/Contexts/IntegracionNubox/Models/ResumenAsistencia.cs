using Integracion.Nubox.Api.Common.Entities;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class ResumenAsistencia : BaseEntity<Guid>
    {
        public Guid TrabajadorId { get; set; }
        public Trabajador Trabajador { get; set; } = null!;
        public int Año { get; set; }
        public int Mes { get; set; }
        public int? Semana { get; set; }
        public int? Quincena { get; set; }
        public decimal TotalHorasRegulares { get; set; }
        public decimal TotalHorasExtras { get; set; }
        public int DiasAsistencia { get; set; }
        public int DiasInasistencia { get; set; }
        public int DiasTardanza { get; set; }
        public int DiasLicencia { get; set; }
        public int DiasVacaciones { get; set; }
        public bool EsProcesado { get; set; } = false;
        public DateTime? FechaProcesamiento { get; set; }
        public Guid? LiquidacionId { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
        public decimal PorcentajeAsistencia => DiasAsistencia + DiasInasistencia > 0
            ? (decimal)DiasAsistencia / (DiasAsistencia + DiasInasistencia) * 100
            : 0;
    }
}
