using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Common.Entities.Enums;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class TransaccionSincronizacion : BaseEntity<Guid>
    {
        public Guid CompaniaId { get; set; }
        public Compania Compania { get; set; } = null!;
        public string TransactionId { get; set; } = string.Empty;
        public TipoIntegracion TipoIntegracion { get; set; }
        public Sistemas SistemaOrigen { get; set; }
        public EstadoProcesamiento Estado { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }
        public DateTime FechaPeriodo { get; set; }
        public int RegistrosProcesados { get; set; } = 0;
        public int RegistrosTotales { get; set; } = 0;
        public int RegistrosErrores { get; set; } = 0;
        public string? DetalleRequest { get; set; }
        public string? DetalleResponse { get; set; }
        public string? MensajeError { get; set; }
        public long DuracionMs { get; set; } = 0;
        public decimal TamañoArchivoMB { get; set; } = 0;
        public string ProcesadoPor { get; set; } = "System";
    }
}
