using System.ComponentModel.DataAnnotations;

namespace Integracion.Nubox.Api.Common.Entities
{
    public abstract class BaseEntity<TId>
    {
        [Key]
        public required TId Id { get; set; }
    }
}
