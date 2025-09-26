using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Infrastructure.Persistence;

namespace Integracion.Nubox.Api.Features.Asistencia.BackgroundServices
{
    public class AsistenciaProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AsistenciaProcessingService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5);

        public AsistenciaProcessingService(IServiceProvider serviceProvider,
            ILogger<AsistenciaProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var registrosPendientes = await unitOfWork.RegistrosAsistencia.GetPendientesProcesarAsync();

                    if (registrosPendientes.Count > 0)
                    {
                        _logger.LogInformation("Procesando {Count} registros pendientes", registrosPendientes.Count);

                        var ids = registrosPendientes.Select(r => r.Id).ToList();
                        await unitOfWork.RegistrosAsistencia.BulkUpdateEstadoAsync(ids, EstadoProcesamiento.Completado);

                        _logger.LogInformation("Registros procesados exitosamente");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando registros pendientes");
                }

                await Task.Delay(_processingInterval, stoppingToken);
            }
        }
    }
}
