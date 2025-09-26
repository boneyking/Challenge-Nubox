using Integracion.Nubox.Api.Features.Nomina.Requests;

namespace Integracion.Nubox.Api.Features.Nomina.Publishers
{
    public interface ISincronizarNominaPublisher
    {
        Task PublishAsync(NominaRequest request);
    }
}
