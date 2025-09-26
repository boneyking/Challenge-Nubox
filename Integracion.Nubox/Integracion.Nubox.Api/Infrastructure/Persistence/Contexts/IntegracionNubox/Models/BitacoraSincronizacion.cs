using Integracion.Nubox.Api.Common.Entities;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class BitacoraSincronizacion : BaseEntity<Guid>
    {
        public Guid CompaniaId { get; set; }
        public Compania Compania { get; set; } = null!;
        public DateTime FechaSincronizacion { get; set; }
        public string Detalles { get; set; } = string.Empty;
    }
}
