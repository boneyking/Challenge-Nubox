using Integracion.Nubox.Api.Features.Nomina.Requests;
using MediatR;

namespace Integracion.Nubox.Api.Features.Nomina.Endpoints
{
    public static class NominaEndpoints
    {

        public static void AddNominaEndpoints(this WebApplication app)
        {
            app.MapPost("/api/nomina/sincronizar", async (NominaRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("Sincronizar")
            .WithTags("Nomina")
            .Produces(200, typeof(object))
            .Produces(500, typeof(object))
            .RequireAuthorization();


        }
    }
}
