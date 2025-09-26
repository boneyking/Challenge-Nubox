using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth
{
    public class AuthContextFactory : IDesignTimeDbContextFactory<AuthContext>
    {
        private readonly IConfiguration _configuration;

        public AuthContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthContext>();

            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("AuthConnection"));

            return new AuthContext(optionsBuilder.Options, _configuration);
        }
    }
}
