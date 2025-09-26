using AutoFixture;
using AutoFixture.Kernel;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace Integracion.Nubox.Api.Tests.Common
{
    public abstract class TestBase
    {
        protected readonly Fixture Fixture;
        protected readonly Mock<ILogger> MockLogger;
        protected readonly IConfiguration Configuration;

        protected TestBase()
        {
            Fixture = new Fixture();
            MockLogger = new Mock<ILogger>();

            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:IntegracionNuboxConnection"] =
                    "Server=127.0.0.1,1433;Database=IntegracionNubox;User ID=integracionNuboxlogin;Password=Fatel4707!@;TrustServerCertificate=True",
                ["ConnectionStrings:AuthConnection"] =
                    "Server=127.0.0.1,1433;Database=Auth;User ID=authlogin;Password=Fatel4707!@;TrustServerCertificate=True"
            }!);

            Configuration = configurationBuilder.Build();
        }

        protected IntegracionNuboxContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<IntegracionNuboxContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            return new IntegracionNuboxContext(options, Configuration);
        }

        protected Mock<ILogger<T>> CreateMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }
    }

    public class IgnoreVirtualMembersCustomisation : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi && pi.GetGetMethod()?.IsVirtual == true)
            {
                return new OmitSpecimen();
            }
            return new NoSpecimen();
        }
    }
}
