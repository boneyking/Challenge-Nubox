using FluentAssertions;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Repositories;
using Integracion.Nubox.Api.Tests.Common;

namespace Integracion.Nubox.Api.Tests.Units.Repositories
{
    public class TrabajadorRepositoryTests : TestBase, IDisposable
    {
        private readonly IntegracionNuboxContext _context;
        private readonly TrabajadorRepository _repository;

        public TrabajadorRepositoryTests()
        {
            _context = CreateInMemoryContext();
            _repository = new TrabajadorRepository(_context);
        }

        [Fact]
        public async Task GetByDniAndCompaniaAsync_Should_Return_Trabajador_When_Exists()
        {
            // Arrange - Crear objetos sin propiedades de navegación
            var companiaId = Guid.NewGuid();
            var compania = new Compania
            {
                Id = companiaId,
                Nombre = "Compañía Test"
            };

            var trabajadorId = Guid.NewGuid();
            var trabajador = new Trabajador
            {
                Id = trabajadorId,
                CompaniaId = companiaId,
                Dni = "12345678",
                Nombres = "Juan",
                Apellidos = "Pérez",
                EsActivo = true
                // NO asignar la propiedad de navegación Compania
            };

            await _context.Companias.AddAsync(compania);
            await _context.Trabajadores.AddAsync(trabajador);
            await _context.SaveChangesAsync();

            // Limpiar el tracker para simular una nueva consulta
            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetByDniAndCompaniaAsync("12345678", companiaId);

            // Assert
            result.Should().NotBeNull();
            result!.Dni.Should().Be("12345678");
            result.CompaniaId.Should().Be(companiaId);
        }

        [Fact]
        public async Task GetByDniAndCompaniaAsync_Should_Return_Null_When_Not_Exists()
        {
            var companiaId = Guid.NewGuid();
            var compania = new Compania
            {
                Id = companiaId,
                Nombre = "Compañía Test"
            };

            var trabajadorId = Guid.NewGuid();
            var trabajador = new Trabajador
            {
                Id = trabajadorId,
                CompaniaId = companiaId,
                Dni = "12345678",
                Nombres = "Juan",
                Apellidos = "Pérez",
                EsActivo = true
                // NO asignar la propiedad de navegación Compania
            };

            await _context.Companias.AddAsync(compania);
            await _context.Trabajadores.AddAsync(trabajador);
            await _context.SaveChangesAsync();


            // Arrange
            var dni = "99999999";

            // Act
            var result = await _repository.GetByDniAndCompaniaAsync(dni, companiaId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByCompaniaAsync_Should_Return_Only_Active_Workers_When_SoloActivos_True()
        {
            // Arrange
            var companiaId = Guid.NewGuid();
            var compania = new Compania
            {
                Id = companiaId,
                Nombre = "Compañía Test"
            };

            var trabajadorActivo = new Trabajador
            {
                Id = Guid.NewGuid(),
                CompaniaId = companiaId,
                Dni = "11111111",
                Nombres = "Pedro",
                Apellidos = "García",
                EsActivo = true
            };

            var trabajadorInactivo = new Trabajador
            {
                Id = Guid.NewGuid(),
                CompaniaId = companiaId,
                Dni = "22222222",
                Nombres = "María",
                Apellidos = "López",
                EsActivo = false
            };

            await _context.Companias.AddAsync(compania);
            await _context.Trabajadores.AddRangeAsync(trabajadorActivo, trabajadorInactivo);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetByCompaniaAsync(companiaId, soloActivos: true);

            // Assert
            result.Should().HaveCount(1);
            result.First().EsActivo.Should().BeTrue();
            result.First().Dni.Should().Be("11111111");
        }

        [Fact]
        public async Task GetByCompaniaAsync_Should_Return_All_Workers_When_SoloActivos_False()
        {
            // Arrange
            var companiaId = Guid.NewGuid();
            var compania = new Compania
            {
                Id = companiaId,
                Nombre = "Compañía Test"
            };

            var trabajadores = new List<Trabajador>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CompaniaId = companiaId,
                Dni = "33333333",
                Nombres = "Ana",
                Apellidos = "Martínez",
                EsActivo = true
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompaniaId = companiaId,
                Dni = "44444444",
                Nombres = "Luis",
                Apellidos = "Rodríguez",
                EsActivo = false
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompaniaId = companiaId,
                Dni = "55555555",
                Nombres = "Carmen",
                Apellidos = "Fernández",
                EsActivo = true
            }
        };

            await _context.Companias.AddAsync(compania);
            await _context.Trabajadores.AddRangeAsync(trabajadores);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // Act
            var result = await _repository.GetByCompaniaAsync(companiaId, soloActivos: false);

            // Assert
            result.Should().HaveCount(3);
            result.Count(t => t.EsActivo).Should().Be(2);
            result.Count(t => !t.EsActivo).Should().Be(1);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
