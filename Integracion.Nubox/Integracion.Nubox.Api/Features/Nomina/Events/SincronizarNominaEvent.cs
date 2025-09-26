using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Features.Nomina.Requests;
using MediatR;

namespace Integracion.Nubox.Api.Features.Nomina.Events
{
    public class SincronizarNominaEvent : Event, IRequest<Response>
    {
        public override string NombreEvento => nameof(SincronizarNominaEvent);
        public override string Exchange => "Nomina";
        public NominaRequest Nomina { get; set; } = new NominaRequest();
    }
}