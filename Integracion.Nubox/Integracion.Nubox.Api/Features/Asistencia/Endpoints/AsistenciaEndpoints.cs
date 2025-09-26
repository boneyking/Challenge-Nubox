using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Features.Asistencia.Services;
using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Features.Asistencia.Endpoints
{
    public static class AsistenciaEndpoints
    {
        public static void AddAsistenciaEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/asistencia")
                .WithTags("Asistencia")
                .RequireAuthorization();

            group.MapPost("/upload-excel/{companiaId:guid}", async (
                Guid companiaId,
                HttpRequest request,
                IServiceProvider serviceProvider) =>
                        {
                            if (!request.HasFormContentType)
                                return Results.BadRequest("Content-Type debe ser multipart/form-data");

                            var form = await request.ReadFormAsync();

                            if (form.Files.Count == 0)
                                return Results.BadRequest("No se encontró archivo");

                            var file = form.Files[0];

                            if (file == null || file.Length == 0)
                                return Results.BadRequest("No se encontró archivo");

                            var memoryStream = new MemoryStream();
                            await file.CopyToAsync(memoryStream);
                            memoryStream.Position = 0;

                            var fileInMemory = new FormFile(
                                memoryStream,
                                0,
                                memoryStream.Length,
                                file.Name,
                                file.FileName)
                            {
                                Headers = file.Headers,
                                ContentType = file.ContentType
                            };

                            var transactionId = Guid.NewGuid().ToString();

                            using (var scope = serviceProvider.CreateScope())
                            {
                                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                                var transaccion = new TransaccionSincronizacion
                                {
                                    Id = Guid.NewGuid(),
                                    CompaniaId = companiaId,
                                    TransactionId = transactionId,
                                    TipoIntegracion = TipoIntegracion.Excel,
                                    SistemaOrigen = Sistemas.ControlAsistencia,
                                    Estado = EstadoProcesamiento.Pendiente,
                                    FechaPeriodo = DateTime.Now.Date,
                                    TamañoArchivoMB = file.Length / (1024m * 1024m),
                                    DetalleRequest = $"Archivo: {file.FileName}"
                                };

                                await unitOfWork.TransaccionesSincronizacion.AddAsync(transaccion);
                                await unitOfWork.SaveChangesAsync();
                            }

                            _ = Task.Run(async () =>
                            {
                                using var scope = serviceProvider.CreateScope();
                                var processorService = scope.ServiceProvider.GetRequiredService<IAsistenciaProcessorService>();

                                try
                                {
                                    await processorService.ProcessExcelFileAsync(fileInMemory, companiaId, transactionId);
                                }
                                catch (Exception ex)
                                {
                                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                                    logger.LogError(ex, "Error en procesamiento background para transacción {TransactionId}", transactionId);
                                }
                                finally
                                {
                                    await memoryStream.DisposeAsync();
                                }
                            });

                            return Results.Accepted($"/api/asistencia/transaction/{transactionId}", new { TransactionId = transactionId });
                        })
                        .DisableAntiforgery()
                        .WithName("UploadExcelAsistencia")
                        .Accepts<IFormFile>("multipart/form-data")
                        .Produces(202)
                        .Produces(400);


            group.MapGet("/transaction/{transactionId}", async (
                string transactionId,
                IUnitOfWork unitOfWork) =>
            {
                var transaccion = await unitOfWork.TransaccionesSincronizacion.GetByTransactionIdAsync(transactionId);

                if (transaccion == null)
                    return Results.NotFound();

                var response = new
                {
                    transaccion.TransactionId,
                    transaccion.Estado,
                    transaccion.FechaInicio,
                    transaccion.FechaFin,
                    transaccion.RegistrosProcesados,
                    transaccion.RegistrosTotales,
                    transaccion.RegistrosErrores,
                    transaccion.MensajeError,
                    transaccion.DuracionMs,
                    Progreso = transaccion.RegistrosTotales > 0
                        ? (decimal)transaccion.RegistrosProcesados / transaccion.RegistrosTotales * 100
                        : 0
                };

                return Results.Ok(response);
            })
            .WithName("GetTransactionStatus")
            .Produces<object>(200)
            .Produces(404);

            group.MapGet("/registros/{companiaId:guid}", async (
                Guid companiaId,
                DateTime? fechaDesde,
                DateTime? fechaHasta,
                IUnitOfWork unitOfWork) =>
            {
                var desde = fechaDesde ?? DateTime.Now.Date.AddDays(-30);
                var hasta = fechaHasta ?? DateTime.Now.Date;

                var registros = await unitOfWork.RegistrosAsistencia.GetByCompaniaYPeriodoAsync(companiaId, desde, hasta);

                var response = registros.Select(r => new
                {
                    r.Id,
                    TrabajadorNombre = $"{r.Trabajador.Nombres} {r.Trabajador.Apellidos}",
                    r.Trabajador.Dni,
                    r.Fecha,
                    r.HoraEntrada,
                    r.HoraSalida,
                    r.HorasRegulares,
                    r.HorasExtras,
                    r.Tipo,
                    r.Observaciones,
                    r.Estado
                });

                return Results.Ok(response);
            })
            .WithName("GetRegistrosAsistencia")
            .Produces<object>(200);

            group.MapGet("/resumenes/{companiaId:guid}", async (
                Guid companiaId,
                int año,
                int mes,
                int? semana,
                int? quincena,
                IResumenAsistenciaService resumenService) =>
            {
                var resumenes = await resumenService.GetResumenesParaLiquidacionAsync(companiaId, año, mes, semana, quincena);

                var response = resumenes.Select(r => new
                {
                    r.Id,
                    TrabajadorNombre = $"{r.Trabajador.Nombres} {r.Trabajador.Apellidos}",
                    r.Trabajador.Dni,
                    r.Año,
                    r.Mes,
                    r.Semana,
                    r.Quincena,
                    r.TotalHorasRegulares,
                    r.TotalHorasExtras,
                    r.DiasAsistencia,
                    r.DiasInasistencia,
                    r.DiasTardanza,
                    r.DiasLicencia,
                    r.DiasVacaciones,
                    r.PorcentajeAsistencia,
                    r.EsProcesado
                });

                return Results.Ok(response);
            })
            .WithName("GetResumenesAsistencia")
            .Produces<object>(200);

            group.MapPost("/procesar/{companiaId:guid}", async (
                Guid companiaId,
                List<AsistenciaDataDto> asistenciaData,
                IAsistenciaProcessorService processorService,
                IUnitOfWork unitOfWork) =>
            {
                var transactionId = Guid.NewGuid().ToString();

                var transaccion = new TransaccionSincronizacion
                {
                    Id = Guid.NewGuid(),
                    CompaniaId = companiaId,
                    TransactionId = transactionId,
                    TipoIntegracion = TipoIntegracion.API,
                    SistemaOrigen = Sistemas.ControlAsistencia,
                    Estado = EstadoProcesamiento.Pendiente,
                    FechaPeriodo = DateTime.Now.Date,
                    RegistrosTotales = asistenciaData.Count,
                    DetalleRequest = System.Text.Json.JsonSerializer.Serialize(asistenciaData.Take(3))
                };

                await unitOfWork.TransaccionesSincronizacion.AddAsync(transaccion);
                await unitOfWork.SaveChangesAsync();

                _ = Task.Run(async () =>
                {
                    await processorService.ProcessAsistenciaDataAsync(asistenciaData, companiaId, transactionId);
                });

                return Results.Accepted($"/api/asistencia/transaction/{transactionId}", new { TransactionId = transactionId });
            })
            .WithName("ProcesarAsistenciaDirecta")
            .Produces(202)
            .Produces(400);

            group.MapGet("/health", async (IUnitOfWork unitOfWork) =>
            {
                var transaccionesPendientes = await unitOfWork.TransaccionesSincronizacion.GetTransaccionesPendientesAsync();
                var registrosPendientes = await unitOfWork.RegistrosAsistencia.GetPendientesProcesarAsync();

                var response = new
                {
                    Status = "Healthy",
                    TransaccionesPendientes = transaccionesPendientes.Count,
                    RegistrosPendientes = registrosPendientes.Count,
                    Timestamp = DateTime.UtcNow
                };

                return Results.Ok(response);
            })
            .WithName("AsistenciaHealth")
            .Produces<object>(200)
            .AllowAnonymous();
        }
    }
}
