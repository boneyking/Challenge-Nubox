using AutoFixture;
using FluentAssertions;
using Integracion.Nubox.Api.Features.Asistencia.Services;
using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace Integracion.Nubox.Api.Tests.Units.Services
{
    public class AsistenciaValidatorServiceTests : TestBase
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<AsistenciaValidatorService>> _mockLogger;
        private readonly AsistenciaValidatorService _service;

        public AsistenciaValidatorServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<AsistenciaValidatorService>>();
            _service = new AsistenciaValidatorService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ValidateSingleAsync_Should_Return_Error_When_Dni_Is_Empty()
        {
            var asistencia = Fixture.Build<AsistenciaDataDto>()
                .With(a => a.DniTrabajador, "")
                .Create();

            var result = await _service.ValidateSingleAsync(asistencia);

            result.Should().HaveCount(1);
            result.First().Field.Should().Be("DniTrabajador");
            result.First().Error.Should().Be("DNI es requerido");
        }

        [Fact]
        public async Task ValidateSingleAsync_Should_Return_Error_When_Date_Is_Future()
        {
            var asistencia = Fixture.Build<AsistenciaDataDto>()
                .With(a => a.DniTrabajador, "12345678")
                .With(a => a.Fecha, DateTime.Now.AddDays(1))
                .Create();

            var result = await _service.ValidateSingleAsync(asistencia);

            result.Should().Contain(r => r.Field == "Fecha" && r.Error == "La fecha no puede ser futura");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(25)]
        public async Task ValidateSingleAsync_Should_Return_Error_When_HorasRegulares_Invalid(decimal horas)
        {
            var asistencia = Fixture.Build<AsistenciaDataDto>()
                .With(a => a.DniTrabajador, "12345678")
                .With(a => a.Fecha, DateTime.Now.AddDays(-1))
                .With(a => a.HorasRegulares, horas)
                .Create();

            var result = await _service.ValidateSingleAsync(asistencia);
            result.Should().Contain(r => r.Field == "HorasRegulares");
        }

        [Fact]
        public async Task ValidateSingleAsync_Should_Pass_With_Valid_Data()
        {
            var asistencia = Fixture.Build<AsistenciaDataDto>()
                .With(a => a.DniTrabajador, "12345678")
                .With(a => a.Fecha, DateTime.Now.AddDays(-1))
                .With(a => a.HorasRegulares, 8m)
                .With(a => a.HorasExtras, 2m)
                .Create();

            var result = await _service.ValidateSingleAsync(asistencia);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateAsync_Should_Validate_All_Items()
        {
            var asistenciaList = new List<AsistenciaDataDto>
            {
                Fixture.Build<AsistenciaDataDto>()
                    .With(a => a.DniTrabajador, "")
                    .Create(),
                Fixture.Build<AsistenciaDataDto>()
                    .With(a => a.DniTrabajador, "12345678")
                    .With(a => a.Fecha, DateTime.Now.AddDays(-1))
                    .With(a => a.HorasRegulares, 8m)
                    .Create()
            };

            var result = await _service.ValidateAsync(asistenciaList);
            result.Should().HaveCount(1);
        }
    }
}
