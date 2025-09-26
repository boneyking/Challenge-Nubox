using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox
{
    public class IntegracionNuboxContextFactory : IDesignTimeDbContextFactory<IntegracionNuboxContext>
    {
        private readonly IConfiguration _configuration;

        public IntegracionNuboxContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IntegracionNuboxContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegracionNuboxContext>();

            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("IntegracionNubox"));

            return new IntegracionNuboxContext(optionsBuilder.Options, _configuration);
        }
    }
}
