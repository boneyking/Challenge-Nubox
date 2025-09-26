using FluentAssertions;
using Integracion.Nubox.Api.Tests.Common;
using System.Text;
using System.Text.Json;

namespace Integracion.Nubox.Api.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class ApiIntegrationTestsWithCustomHost : IClassFixture<TestHostBuilder>
    {
        private readonly TestHostBuilder _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTestsWithCustomHost(TestHostBuilder factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Health_Endpoint_Should_Return_Success()
        {
            var response = await _client.GetAsync("/health");
            response.Should().BeSuccessful();
        }

        [Fact]
        public async Task Auth_Login_Should_Work_With_Seeded_Data()
        {
            var loginRequest = new
            {
                Username = "admin",
                Password = "hashed_password"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            response.Should().BeSuccessful();
        }

        [Fact]
        public async Task AsistenciaHealth_Should_Return_Healthy_Status()
        {
            var response = await _client.GetAsync("/api/asistencia/health");
            response.Should().BeSuccessful();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Healthy");
        }
    }
}
