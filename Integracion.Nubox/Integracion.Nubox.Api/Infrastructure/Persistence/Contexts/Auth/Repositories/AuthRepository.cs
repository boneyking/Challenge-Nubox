using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Repositories
{
    public class AuthRepository : Repository<User, Guid>, IAuthRepository
    {
        public AuthRepository(AuthContext context) : base(context)
        {
        }

        public async Task<User?> CheckUser(string username, string password) => 
            await Entities
                .FirstOrDefaultAsync(x => x.Username.Equals(username)
                    && x.PasswordHash.Equals(password));
    }

}
