using Integracion.Nubox.Api.Common.Entities;
using MediatR;

namespace Integracion.Nubox.Api.Features.Auth.Requests
{
    public class AuthRequest: IRequest<Response>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
