using Integracion.Nubox.Api.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Integracion.Nubox.Api.Features.Asistencia.HealthChecks
{
    public class AsistenciaHealthCheck : IHealthCheck
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AsistenciaHealthCheck> _logger;

        public AsistenciaHealthCheck(IServiceProvider serviceProvider, 
            ILogger<AsistenciaHealthCheck> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

                if (unitOfWork == null)
                    return HealthCheckResult.Unhealthy("UnitOfWork no está disponible");

                var transaccionesPendientes = await unitOfWork.TransaccionesSincronizacion.GetTransaccionesPendientesAsync();

                if (transaccionesPendientes.Count > 10)
                    return HealthCheckResult.Degraded($"Hay {transaccionesPendientes.Count} transacciones pendientes");

                var registrosPendientes = await unitOfWork.RegistrosAsistencia.GetPendientesProcesarAsync();

                if (registrosPendientes.Count > 1000)
                    return HealthCheckResult.Degraded($"Hay {registrosPendientes.Count} registros pendientes de procesar");

                return HealthCheckResult.Healthy($"Servicio funcionando correctamente. Transacciones pendientes: {transaccionesPendientes.Count}, Registros pendientes: {registrosPendientes.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando salud del servicio de asistencia");
                return HealthCheckResult.Unhealthy($"Error en servicio de asistencia: {ex.Message}");
            }
        }
    }
}
