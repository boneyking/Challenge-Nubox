using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.Auth.Models;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Integracion.Nubox.Api.Tests.Common
{
    public class TestHostBuilder : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                RemoveService<DbContextOptions<IntegracionNuboxContext>>(services);
                RemoveService<DbContextOptions<AuthContext>>(services);
                RemoveService<IntegracionNuboxContext>(services);
                RemoveService<AuthContext>(services);

                services.AddDbContext<IntegracionNuboxContext>(options =>
                    options.UseInMemoryDatabase($"TestIntegracionDb_{Guid.NewGuid()}"));

                services.AddDbContext<AuthContext>(options =>
                    options.UseInMemoryDatabase($"TestAuthDb_{Guid.NewGuid()}"));

                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

                SeedTestData(services);
            });

            builder.UseEnvironment("Testing");
        }

        private static void RemoveService<T>(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
            if (descriptor != null)
                services.Remove(descriptor);
        }

        private static void SeedTestData(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var authContext = scope.ServiceProvider.GetRequiredService<AuthContext>();
            var integracionContext = scope.ServiceProvider.GetRequiredService<IntegracionNuboxContext>();

            if (!authContext.Users.Any())
            {
                authContext.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = "hashed_password",
                    IsActive = true
                });
                authContext.SaveChanges();
            }

            if (!integracionContext.Companias.Any())
            {
                var compania = new Compania
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Test Company"
                };

                integracionContext.Companias.Add(compania);
                integracionContext.SaveChanges();
            }
        }
    }
}
