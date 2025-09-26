using Integracion.Nubox.Api.Common.Entities.Enums;

namespace Integracion.Nubox.Api.Features.Asistencia.Services.Dto
{
    public class AsistenciaDataDto
    {
        public Guid TrabajadorId { get; set; }
        public string DniTrabajador { get; set; } = string.Empty;
        public string? NombresTrabajador { get; set; }
        public string? ApellidosTrabajador { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public decimal HorasRegulares { get; set; }
        public decimal HorasExtras { get; set; }
        public TipoAsistencia Tipo { get; set; }
        public string? Observaciones { get; set; }
        public string? IdExternoPartner { get; set; }
    }
}
