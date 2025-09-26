using Integracion.Nubox.Api.Features.Asistencia.Services;
using Integracion.Nubox.Api.Infrastructure.Persistence;

namespace Integracion.Nubox.Api.Features.Asistencia.BackgroundServices
{
    public class ResumenGeneratorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ResumenGeneratorService> _logger;
        private readonly TimeSpan _generationInterval = TimeSpan.FromHours(6);

        public ResumenGeneratorService(IServiceProvider serviceProvider,
            ILogger<ResumenGeneratorService> logger)
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
                    var resumenService = scope.ServiceProvider.GetRequiredService<IResumenAsistenciaService>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var companias = await unitOfWork.Companias.GetAllAsync();
                    var empresasActivas = companias.ToList();

                    var fechaHoy = DateTime.Now.Date;
                    var fechaDesde = fechaHoy.AddDays(-7);

                    foreach (var compania in empresasActivas)
                    {
                        try
                        {
                            await resumenService.GenerateResumenesAsync(compania.Id, fechaDesde, fechaHoy);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error generando resúmenes para compañía {CompaniaId}", compania.Id);
                        }
                    }

                    _logger.LogInformation("Resúmenes generados para {Count} compañías", empresasActivas.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en generación automática de resúmenes");
                }

                await Task.Delay(_generationInterval, stoppingToken);
            }
        }
    }
}
