using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Features.Auth.Requests;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Repositories;
using MediatR;
using System.IdentityModel.Tokens.Jwt;

namespace Integracion.Nubox.Api.Features.Auth.Handlers
{
    public class AuthHandler(IAuthRepository authRepository,
        ILogger<AuthHandler> logger,
        IConfiguration configuration) : IRequestHandler<AuthRequest, Response>
    {
        public async Task<Response> Handle(AuthRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await authRepository.CheckUser(request.Username, request.Password);
                if (user is null)
                    return new Response
                    {
                        Status = false,
                        Message = "Invalid username or password",
                    };

                var tokenUser = user.CreateToken(user, configuration);
                return new Response
                {
                    Status = true,
                    Message = "OK",
                    Data = new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(tokenUser),
                        Expires = tokenUser.ValidTo,
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AuthHandler");
                throw;
            }
        }
    }
}
