using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Features.Nomina.Events;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Integracion.Nubox.Api.Infrastructure.Services;
using MediatR;

public class SincronizarNominaEventHandler : IRequestHandler<SincronizarNominaEvent, Response>
{
    private readonly ILogger<SincronizarNominaEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBulkInsertService _bulkInsertService;
    private readonly IEfBulkService _efBulkService;

    public SincronizarNominaEventHandler(
        ILogger<SincronizarNominaEventHandler> logger,
        IUnitOfWork unitOfWork,
        IBulkInsertService bulkInsertService,
        IEfBulkService efBulkService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _bulkInsertService = bulkInsertService;
        _efBulkService = efBulkService;
    }

    public async Task<Response> Handle(SincronizarNominaEvent request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing SincronizarNominaEvent for company {CompanyId}",
                request.Nomina.IdCompania);

            var compania = await _unitOfWork.Companias.GetByIdAsync(request.Nomina.IdCompania);
            if (compania is null)
                return new Response
                {
                    Status = false,
                    Message = "Compañía no encontrada",
                    Data = new { }
                };

            var trabajadores = request.Nomina.Trabajadores;

            if (trabajadores.Count == 0)
                return new Response
                {
                    Status = true,
                    Message = "No hay trabajadores para procesar",
                    Data = new { TrabajadoresProcesados = 0 }
                };

            var bitacoras = trabajadores.Select(x => new BitacoraSincronizacion
            {
                Id = Guid.NewGuid(),
                CompaniaId = request.Nomina.IdCompania,
                FechaSincronizacion = DateTime.UtcNow,
                Detalles = $"Trabajador: {x.NombresTrabajador} {x.ApellidosTrabajador}, Dni: {x.DniTrabajador}",

            }).ToList();

            if (bitacoras.Count > 10000)
                await _bulkInsertService.BulkInsertAsync(bitacoras, "BitacoraSincronizacion", cancellationToken);
            else if (bitacoras.Count > 1000)
                await _efBulkService.BulkInsertWithEfAsync(bitacoras, 1000, cancellationToken);
            else
                await _efBulkService.BulkInsertWithEfAsync(bitacoras, 100, cancellationToken);


            _logger.LogInformation("Bulk operation completed successfully");

            return new Response
            {
                Status = true,
                Message = "Carga masiva completada exitosamente",
                Data = new { TrabajadoresProcesados = trabajadores.Count }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during bulk operation");

            return new Response
            {
                Status = false,
                Message = $"Error en carga masiva: {ex.Message}",
                Data = new { }
            };
        }
    }
}