using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public interface IExcelProcessorService
    {
        Task<List<AsistenciaDataDto>> ExtractAsistenciaFromExcelAsync(IFormFile file, Guid companiaId);
        Task<ValidationResult> ValidateExcelFormatAsync(IFormFile file);
    }
}
