using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Infrastructure.Persistence;

namespace Integracion.Nubox.Api.Features.Asistencia.BackgroundServices
{
    public class TransactionMonitoringService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TransactionMonitoringService> _logger;
        private readonly TimeSpan _monitoringInterval = TimeSpan.FromMinutes(10);

        public TransactionMonitoringService(IServiceProvider serviceProvider, ILogger<TransactionMonitoringService> logger)
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

                    var transaccionesPendientes = await unitOfWork.TransaccionesSincronizacion.GetTransaccionesPendientesAsync();

                    foreach (var transaccion in transaccionesPendientes)
                    {
                        _logger.LogWarning("Transacción colgada detectada: {TransactionId}, iniciada: {FechaInicio}",
                            transaccion.TransactionId, transaccion.FechaInicio);
                        transaccion.Estado = EstadoProcesamiento.Error;
                        transaccion.MensajeError = "Transacción expirada por timeout";
                        transaccion.FechaFin = DateTime.UtcNow;
                        unitOfWork.TransaccionesSincronizacion.Update(transaccion);
                    }

                    if (transaccionesPendientes.Count > 0)
                    {
                        await unitOfWork.SaveChangesAsync();
                        _logger.LogInformation("Marcadas {Count} transacciones como expiradas", transaccionesPendientes.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error monitoreando transacciones");
                }

                await Task.Delay(_monitoringInterval, stoppingToken);
            }
        }
    }
}
