using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public interface INotificationService
    {
        Task NotifyProcessingCompletedAsync(Guid companiaId, ProcessingResult result);
        Task NotifyProcessingErrorAsync(Guid companiaId, string errorMessage);
        Task NotifyValidationErrorsAsync(Guid companiaId, List<ValidationError> errors);
    }
}