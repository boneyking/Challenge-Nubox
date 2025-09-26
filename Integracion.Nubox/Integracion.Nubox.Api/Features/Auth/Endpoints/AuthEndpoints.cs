using Integracion.Nubox.Api.Features.Auth.Requests;
using MediatR;

namespace Integracion.Nubox.Api.Features.Auth.Endpoints
{
    public static class AuthEndpoints
    {
        public static void AddAuthEndpoints(this WebApplication app)
        {

            app.MapPost("api/auth/login", async (AuthRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(request, cancellationToken);
                return Results.Ok(result);
            });

        }

    }
}
