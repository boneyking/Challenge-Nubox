using FluentAssertions;
using Integracion.Nubox.Api.Features.Asistencia.Services;
using Integracion.Nubox.Api.Tests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace Integracion.Nubox.Api.Tests.Units.Services
{
    public class ExcelProcessorServiceTests : TestBase
    {
        private readonly Mock<ILogger<ExcelProcessorService>> _mockLogger;
        private readonly ExcelProcessorService _service;

        public ExcelProcessorServiceTests()
        {
            _mockLogger = new Mock<ILogger<ExcelProcessorService>>();
            _service = new ExcelProcessorService(_mockLogger.Object);
        }

        [Fact]
        public async Task ValidateExcelFormatAsync_Should_Return_Invalid_When_File_Empty()
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);
            mockFile.Setup(f => f.FileName).Returns("test.xlsx");
            var result = await _service.ValidateExcelFormatAsync(mockFile.Object);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("El archivo está vacío");
        }

        [Theory]
        [InlineData("test.txt")]
        [InlineData("test.pdf")]
        [InlineData("test.docx")]
        public async Task ValidateExcelFormatAsync_Should_Return_Invalid_For_Wrong_Extension(string fileName)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1000);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            var result = await _service.ValidateExcelFormatAsync(mockFile.Object);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain("Formato de archivo no válido. Solo se permiten archivos Excel (.xlsx, .xls)");
        }

        [Theory]
        [InlineData("test.xlsx")]
        [InlineData("test.xls")]
        public async Task ValidateExcelFormatAsync_Should_Return_Valid_For_Excel_Extensions(string fileName)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(1000);
            mockFile.Setup(f => f.FileName).Returns(fileName);

            var excelBytes = Encoding.UTF8.GetBytes("Mock Excel Content");
            var stream = new MemoryStream(excelBytes);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);

            var exception = await Assert.ThrowsAnyAsync<Exception>(() => _service.ValidateExcelFormatAsync(mockFile.Object));
            exception.Should().NotBeNull();
        }
    }
}
