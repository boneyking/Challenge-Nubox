using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Common.Entities.Enums;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class ConfiguracionPartner : BaseEntity<Guid>
    {
        public Guid CompaniaId { get; set; }
        public Compania Compania { get; set; } = null!;
        public string NombrePartner { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool EsActivo { get; set; } = true;
        public TipoIntegracion TipoIntegracionPreferido { get; set; } = TipoIntegracion.API;
        public TimeSpan IntervaloSincronizacion { get; set; } = TimeSpan.FromHours(1);
        public DateTime? UltimaSincronizacion { get; set; }
        public DateTime? ProximaSincronizacion { get; set; }
        public bool SincronizacionAutomatica { get; set; } = true;
        public bool NotificarErrores { get; set; } = true;
        public string? EmailNotificaciones { get; set; }
        public int TimeoutSegundos { get; set; } = 30;
        public int MaxReintentos { get; set; } = 3;
        public int TamañoLoteMaximo { get; set; } = 1000;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
    }
}
