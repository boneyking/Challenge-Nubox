using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Features.Nomina.Publishers;
using Integracion.Nubox.Api.Features.Nomina.Requests;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using MediatR;

namespace Integracion.Nubox.Api.Features.Nomina.Handlers
{
    public class SincronizarNominaHandler : IRequestHandler<NominaRequest, Response>
    {
        private readonly ILogger<SincronizarNominaHandler> _logger;
        private readonly ISincronizarNominaPublisher _sincronizarNominaPublisher;
        private readonly IUnitOfWork _unitOfWork;
        public SincronizarNominaHandler(ILogger<SincronizarNominaHandler> logger, 
            ISincronizarNominaPublisher sincronizarNominaPublisher,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _sincronizarNominaPublisher = sincronizarNominaPublisher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response> Handle(NominaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing SincronizarNominaEvent for company {CompanyId}",
                    request.IdCompania);

                var compania = await _unitOfWork.Companias.GetByIdAsync(request.IdCompania);
                if (compania is null)
                    return new Response
                    {
                        Status = false,
                        Message = "Compañía no encontrada",
                        Data = new { }
                    };


                await _sincronizarNominaPublisher.PublishAsync(request);

                return new Response
                {
                    Status = true,
                    Message = "Nómina sincronizada exitosamentess",
                    Data = new { TrabajadoresProcesados = request.Trabajadores.Count }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SincronizarNominaEvent");

                return new Response
                {
                    Status = false,
                    Message = $"Error al procesar la sincronización: {ex.Message}",
                    Data = new { }
                };
            }
        }
    }
}
