using FluentAssertions;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;
using Microsoft.AspNetCore.Mvc.Testing;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Integracion.Nubox.Api.Tests.Integration
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<IntegracionNuboxContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<IntegracionNuboxContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Health_Endpoint_Should_Return_Success()
        {
            var response = await _client.GetAsync("/health");
            response.Should().BeSuccessful();
        }

        [Fact]
        public async Task Swagger_Should_Be_Accessible()
        {
            var response = await _client.GetAsync("/swagger/index.html");
            response.Should().BeSuccessful();
        }

        [Fact]
        public async Task AsistenciaHealth_Should_Return_Success()
        {
            var response = await _client.GetAsync("/api/asistencia/health");
            response.Should().BeSuccessful();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Healthy");
        }
    }
}
