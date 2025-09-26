using AutoFixture;
using FluentAssertions;
using Integracion.Nubox.Api.Features.Nomina.Handlers;
using Integracion.Nubox.Api.Features.Nomina.Publishers;
using Integracion.Nubox.Api.Features.Nomina.Requests;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories;
using Integracion.Nubox.Api.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace Integracion.Nubox.Api.Tests.Units.Handlers
{
    public class SincronizarNominaHandlerTests : TestBase
    {
        private readonly Mock<ILogger<SincronizarNominaHandler>> _mockLogger;
        private readonly Mock<ISincronizarNominaPublisher> _mockPublisher;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICompaniaRepository> _mockCompaniaRepository;
        private readonly SincronizarNominaHandler _handler;

        public SincronizarNominaHandlerTests()
        {
            _mockLogger = new Mock<ILogger<SincronizarNominaHandler>>();
            _mockPublisher = new Mock<ISincronizarNominaPublisher>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCompaniaRepository = new Mock<ICompaniaRepository>();

            _mockUnitOfWork.Setup(u => u.Companias).Returns(_mockCompaniaRepository.Object);

            _handler = new SincronizarNominaHandler(
                _mockLogger.Object,
                _mockPublisher.Object,
                _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Error_When_Company_Not_Found()
        {
            var request = Fixture.Create<NominaRequest>();
            _mockCompaniaRepository.Setup(r => r.GetByIdAsync(request.IdCompania))
                .ReturnsAsync((Compania?)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.Status.Should().BeFalse();
            result.Message.Should().Be("Compañía no encontrada");
        }

        [Fact]
        public async Task Handle_Should_Publish_And_Return_Success_When_Company_Exists()
        {
            var compania = Fixture.Create<Compania>();
            var request = Fixture.Build<NominaRequest>()
                .With(r => r.IdCompania, compania.Id)
                .Create();

            _mockCompaniaRepository.Setup(r => r.GetByIdAsync(request.IdCompania))
                .ReturnsAsync(compania);

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.Status.Should().BeTrue();
            result.Message.Should().Be("Nómina sincronizada exitosamentess");
            result.Data.Should().NotBeNull();

            _mockPublisher.Verify(p => p.PublishAsync(request), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Error_When_Publisher_Throws_Exception()
        {
            var compania = Fixture.Create<Compania>();
            var request = Fixture.Build<NominaRequest>()
                .With(r => r.IdCompania, compania.Id)
                .Create();

            _mockCompaniaRepository.Setup(r => r.GetByIdAsync(request.IdCompania))
                .ReturnsAsync(compania);

            _mockPublisher.Setup(p => p.PublishAsync(It.IsAny<NominaRequest>()))
                .ThrowsAsync(new Exception("Publisher error"));

            var result = await _handler.Handle(request, CancellationToken.None);

            result.Should().NotBeNull();
            result.Status.Should().BeFalse();
            result.Message.Should().StartWith("Error al procesar la sincronización:");
        }
    }
}
