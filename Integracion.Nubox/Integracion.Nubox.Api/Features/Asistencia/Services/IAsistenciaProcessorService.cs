using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public interface IAsistenciaProcessorService
    {
        Task<ProcessingResult> ProcessAsistenciaDataAsync(List<AsistenciaDataDto> asistenciaData, Guid companiaId, string transactionId);
        Task<ProcessingResult> ProcessExcelFileAsync(IFormFile file, Guid companiaId, string transactionId);
        Task<List<ValidationError>> ValidateAsistenciaDataAsync(List<AsistenciaDataDto> asistenciaData);
    }
}
