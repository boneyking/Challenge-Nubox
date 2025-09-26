using Integracion.Nubox.Api.Common.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Models
{
    public class User : BaseEntity<Guid>
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; }

        public JwtSecurityToken CreateToken(User user, IConfiguration configuration)
        {
            var authClaims = new List<Claim>
            {
                new("IdUsuario", user.Id.ToString()),
                new("NombreUsuario", user.Username)
            };

            var secret = configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("JWT secret is not configured.");

            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            return new JwtSecurityToken(
                expires: DateTime.Now.AddHours(24),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256));
        }
    }
}
