using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Common.Entities.Enums;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class RegistroAsistencia : BaseEntity<Guid>
    {
        public Guid TrabajadorId { get; set; }
        public Trabajador Trabajador { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public decimal HorasRegulares { get; set; }
        public decimal HorasExtras { get; set; }
        public TipoAsistencia Tipo { get; set; }
        public string? Observaciones { get; set; }
        public string? IdExternoPartner { get; set; }
        public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;
        public EstadoProcesamiento Estado { get; set; } = EstadoProcesamiento.Pendiente;
        public string? ErrorMessage { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        public string CreadoPor { get; set; } = "System";
    }
}
