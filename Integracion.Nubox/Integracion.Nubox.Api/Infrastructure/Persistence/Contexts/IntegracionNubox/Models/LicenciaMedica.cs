using Integracion.Nubox.Api.Common.Entities;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class LicenciaMedica : BaseEntity<Guid>
    {
        public Guid TrabajadorId { get; set; }
        public Trabajador Trabajador { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string TipoLicencia { get; set; } = string.Empty;
        public string NumeroLicencia { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public bool EsAprobada { get; set; } = false;
        public string? IdExternoPartner { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
