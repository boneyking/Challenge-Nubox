using Integracion.Nubox.Api.Common.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Repositories
{
    public interface IAuthRepository : IRepository<User,Guid>
    {
        Task<User?> CheckUser(string username, string password);
    }

}
