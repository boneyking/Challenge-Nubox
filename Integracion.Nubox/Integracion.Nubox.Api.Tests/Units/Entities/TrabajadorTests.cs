using AutoFixture;
using FluentAssertions;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Integracion.Nubox.Api.Tests.Common;

namespace Integracion.Nubox.Api.Tests.Units.Entities
{
    public class TrabajadorTests : TestBase
    {
        [Fact]
        public void Trabajador_Should_Initialize_With_Default_Values()
        {
            var trabajador = new Trabajador
            {
                Id = Guid.NewGuid(),
                CompaniaId = Guid.NewGuid(),
                Nombres = "Juan",
                Apellidos = "Pérez",
                Dni = "12345678",
                Email = "juan.perez@test.com"
            };
            trabajador.EsActivo.Should().BeTrue();
            trabajador.RegistrosAsistencia.Should().NotBeNull().And.BeEmpty();
            trabajador.LicenciasMedicas.Should().NotBeNull().And.BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Trabajador_Should_Handle_Invalid_Names(string invalidName)
        {
            var trabajador = Fixture.Build<Trabajador>()
                .With(t => t.Nombres, invalidName)
                .Create();

            trabajador.Nombres.Should().Be(invalidName);
        }

        [Fact]
        public void Trabajador_Should_Set_FechaIngreso_Before_FechaSalida()
        {
            var fechaIngreso = DateTime.Now.AddYears(-1);
            var fechaSalida = DateTime.Now;

            var trabajador = Fixture.Build<Trabajador>()
                .With(t => t.FechaIngreso, fechaIngreso)
                .With(t => t.FechaSalida, fechaSalida)
                .Create();

            trabajador.FechaIngreso.Should().BeBefore(trabajador.FechaSalida.Value);
        }
    }
}
