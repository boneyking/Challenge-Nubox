using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public interface IAsistenciaValidatorService
    {
        Task<List<ValidationError>> ValidateAsync(List<AsistenciaDataDto> asistenciaData);
        Task<List<ValidationError>> ValidateSingleAsync(AsistenciaDataDto asistencia);
    }
}
