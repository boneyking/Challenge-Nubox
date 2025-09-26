using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Common.Entities.Enums;
using MediatR;

namespace Integracion.Nubox.Api.Features.Nomina.Requests
{
    public class NominaRequest : IRequest<Response>
    {
        public Guid IdCompania { get; set; } = Guid.Empty;        
        public Sistemas Origen { get; set; } = Sistemas.Ninguno;
        public List<InformacionBasicaTrabajador> Trabajadores { get; set; } = [];

    }

    public class InformacionBasicaTrabajador
    {
        public Guid IdTrabajador { get; set; } = Guid.Empty;
        public string NombresTrabajador { get; set; } = string.Empty;
        public string ApellidosTrabajador { get; set; } = string.Empty;
        public string DniTrabajador { get; set; } = string.Empty;
    }
}
