using Integracion.Nubox.Api.Common.Entities;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models
{
    public class Compania : BaseEntity<Guid>
    {
        public string Nombre { get; set; } = string.Empty;
    }
}
