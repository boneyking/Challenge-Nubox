using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;
using Integracion.Nubox.Api.Infrastructure.Persistence;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUnitOfWork unitOfWork,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task NotifyProcessingCompletedAsync(Guid companiaId, ProcessingResult result)
        {
            try
            {
                var configuracion = await _unitOfWork.ConfiguracionesPartner.GetByCompaniaAsync(companiaId);

                if (configuracion?.NotificarErrores == true && !string.IsNullOrEmpty(configuracion.EmailNotificaciones))
                {
                    var mensaje = $"Procesamiento completado exitosamente. " +
                                 $"Registros procesados: {result.ProcessedRecords}. " +
                                 $"Tiempo: {result.ProcessingTimeMs}ms. " +
                                 $"Errores: {result.Errors.Count}";

                    _logger.LogInformation("Notificación de éxito: {Mensaje}", mensaje);
                    await SendEmailAsync(configuracion.EmailNotificaciones,
                        "Notificación de Procesamiento de Asistencia", mensaje);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando notificación de éxito");
            }
        }

        public async Task NotifyProcessingErrorAsync(Guid companiaId, string errorMessage)
        {
            try
            {
                var configuracion = await _unitOfWork.ConfiguracionesPartner.GetByCompaniaAsync(companiaId);

                if (configuracion?.NotificarErrores == true && !string.IsNullOrEmpty(configuracion.EmailNotificaciones))
                {
                    _logger.LogWarning("Error en procesamiento de asistencia para compañía {CompaniaId}: {Error}",
                        companiaId, errorMessage);
                    await SendEmailAsync(configuracion.EmailNotificaciones,
                        "Error en Procesamiento de Asistencia", errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando notificación de error");
            }
        }

        public async Task NotifyValidationErrorsAsync(Guid companiaId, List<ValidationError> errors)
        {
            try
            {
                var configuracion = await _unitOfWork.ConfiguracionesPartner.GetByCompaniaAsync(companiaId);

                if (configuracion?.NotificarErrores == true && !string.IsNullOrEmpty(configuracion.EmailNotificaciones))
                {
                    var mensaje = $"Se encontraron {errors.Count} errores de validación en los datos de asistencia.";
                    _logger.LogWarning("Errores de validación: {Mensaje}", mensaje);
                    await SendEmailAsync(configuracion.EmailNotificaciones,
                        "Errores de Validación en Asistencia", mensaje);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando notificación de errores de validación");
            }

        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            //Solo para simular un envio de email
            _logger.LogInformation("Enviando email a {To} con asunto {Subject}", to, subject);
            _logger.LogInformation("Cuerpo del email: {Body}", body);
            await Task.Delay(500);
            await Task.CompletedTask;
        }
    }
}
