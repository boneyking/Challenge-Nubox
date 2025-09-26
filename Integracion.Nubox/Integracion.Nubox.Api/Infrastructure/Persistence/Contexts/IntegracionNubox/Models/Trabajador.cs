using Integracion.Nubox.Api.Common.Entities;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class Trabajador : BaseEntity<Guid>
    {
        public Guid CompaniaId { get; set; }
        public Compania Compania { get; set; } = null!;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EsActivo { get; set; } = true;
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaSalida { get; set; }
        public string? IdExternoPartner { get; set; }
        public DateTime? UltimaSincronizacion { get; set; }
        public ICollection<RegistroAsistencia> RegistrosAsistencia { get; set; } = [];
        public ICollection<LicenciaMedica> LicenciasMedicas { get; set; } = [];
    }
}
