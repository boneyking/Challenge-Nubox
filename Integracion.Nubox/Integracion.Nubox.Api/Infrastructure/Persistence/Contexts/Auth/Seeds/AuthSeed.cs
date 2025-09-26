using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Seeds
{
    public static class AuthSeed
    {
        public static void Seed(AuthContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = "hashed_password",
                    IsActive = true
                });
                context.SaveChanges();
            }
        }
    }
}
