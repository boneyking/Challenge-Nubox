using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;
using Integracion.Nubox.Api.Infrastructure.Persistence;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;
using Integracion.Nubox.Api.Infrastructure.Services;
using System.Diagnostics;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public class AsistenciaProcessorService : IAsistenciaProcessorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAsistenciaValidatorService _validator;
        private readonly IResumenAsistenciaService _resumenService;
        private readonly IExcelProcessorService _excelProcessor;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AsistenciaProcessorService> _logger;
        private readonly IBulkInsertService _bulkInsertService;
        private readonly IEfBulkService _efBulkService;

        public AsistenciaProcessorService(
            IUnitOfWork unitOfWork,
            IAsistenciaValidatorService validator,
            IResumenAsistenciaService resumenService,
            IExcelProcessorService excelProcessor,
            INotificationService notificationService,
            ILogger<AsistenciaProcessorService> logger,
            IEfBulkService efBulkService,
            IBulkInsertService bulkInsertService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _resumenService = resumenService;
            _excelProcessor = excelProcessor;
            _notificationService = notificationService;
            _logger = logger;
            _efBulkService = efBulkService;
            _bulkInsertService = bulkInsertService;
        }

        public async Task<ProcessingResult> ProcessAsistenciaDataAsync(
            List<AsistenciaDataDto> asistenciaData,
            Guid companiaId,
            string transactionId)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new ProcessingResult { TransactionId = transactionId };

            try
            {
                _logger.LogInformation("Iniciando procesamiento de {Count} registros de asistencia para compañía {CompaniaId}",
                    asistenciaData?.Count ?? 0, companiaId);

                if (asistenciaData is null || asistenciaData.Count == 0)
                {
                    result.Success = false;
                    result.ErrorMessage = "No hay datos para procesar";
                    await UpdateTransactionStatusAsync(transactionId, EstadoProcesamiento.Error, 0, result.ErrorMessage);
                    return result;
                }

                await UpdateTransactionStatusAsync(transactionId, EstadoProcesamiento.Procesando, asistenciaData.Count);

                var validationErrors = await ValidateAsistenciaDataAsync(asistenciaData);
                if (validationErrors.Any())
                {
                    result.Success = false;
                    result.ValidationErrors = validationErrors;
                    result.ErrorMessage = $"Se encontraron {validationErrors.Count} errores de validación";                    
                    await UpdateTransactionStatusAsync(transactionId, EstadoProcesamiento.Error, 0, result.ErrorMessage);
                    await _notificationService.NotifyValidationErrorsAsync(companiaId, validationErrors);
                    _logger.LogInformation("Validación fallida con {ErrorCount} errores", validationErrors.Count);
                    return result;
                }

                var batchSize = 2000;
                var totalProcessed = 0;
                var errors = new List<string>();

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    for (int i = 0; i < asistenciaData.Count; i += batchSize)
                    {
                        var batch = asistenciaData.Skip(i).Take(batchSize).ToList();
                        var batchResult = await ProcessBatchAsync(batch, companiaId);

                        totalProcessed += batchResult.ProcessedCount;
                        errors.AddRange(batchResult.Errors);

                        await UpdateTransactionProgressAsync(transactionId, totalProcessed, errors.Count);

                        _logger.LogDebug("Procesado lote {BatchNumber}: {Processed} registros",
                            (i / batchSize) + 1, batchResult.ProcessedCount);
                    }

                    if (asistenciaData.Count != 0)
                    {
                        var fechaInicio = asistenciaData.Min(a => a.Fecha);
                        var fechaFin = asistenciaData.Max(a => a.Fecha);

                        await _resumenService.GenerateResumenesAsync(companiaId, fechaInicio, fechaFin);
                    }
                    else
                    {
                        _logger.LogWarning("No hay datos válidos para generar resúmenes");
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    result.Success = true;
                    result.ProcessedRecords = totalProcessed;
                    result.Errors = errors;
                    result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                    await UpdateTransactionStatusAsync(transactionId, EstadoProcesamiento.Completado, totalProcessed);

                    _logger.LogInformation("Procesamiento completado exitosamente. {Processed} registros procesados en {Duration}ms",
                        totalProcessed, stopwatch.ElapsedMilliseconds);

                    await _notificationService.NotifyProcessingCompletedAsync(companiaId, result);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante procesamiento de asistencia para transacción {TransactionId}", transactionId);

                result.Success = false;
                result.ErrorMessage = ex.Message;

                await UpdateTransactionStatusAsync(transactionId, EstadoProcesamiento.Error, 0, ex.Message);
                await _notificationService.NotifyProcessingErrorAsync(companiaId, ex.Message);
            }

            return result;
        }

        public async Task<ProcessingResult> ProcessExcelFileAsync(IFormFile file, Guid companiaId, string transactionId)
        {
            try
            {
                _logger.LogInformation("Procesando archivo Excel {FileName} para compañía {CompaniaId}",
                    file.FileName, companiaId);

                var asistenciaData = await _excelProcessor.ExtractAsistenciaFromExcelAsync(file, companiaId);

                return await ProcessAsistenciaDataAsync(asistenciaData, companiaId, transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar archivo Excel {FileName}", file.FileName);

                var result = new ProcessingResult
                {
                    TransactionId = transactionId,
                    Success = false,
                    ErrorMessage = $"Error al procesar archivo Excel: {ex.Message}"
                };

                await UpdateTransactionStatusAsync(transactionId, EstadoProcesamiento.Error, 0, ex.Message);
                return result;
            }
        }

        public async Task<List<ValidationError>> ValidateAsistenciaDataAsync(List<AsistenciaDataDto> asistenciaData)
                => await _validator.ValidateAsync(asistenciaData);

        private async Task<BatchProcessingResult> ProcessBatchAsync(List<AsistenciaDataDto> batch, Guid companiaId)
        {
            var result = new BatchProcessingResult();
            var registros = new List<RegistroAsistencia>();

            foreach (var item in batch)
            {
                try
                {
                    var trabajador = await _unitOfWork.Trabajadores.GetByDniAndCompaniaAsync(item.DniTrabajador, companiaId);
                    trabajador ??= await CreateTrabajadorAsync(item, companiaId);

                    var existeRegistro = await _unitOfWork.RegistrosAsistencia.ExisteRegistroAsync(trabajador.Id, item.Fecha);
                    if (existeRegistro)
                    {
                        result.Errors.Add($"Ya existe un registro de asistencia para {item.DniTrabajador} en fecha {item.Fecha:yyyy-MM-dd}");
                        continue;
                    }

                    var registro = new RegistroAsistencia
                    {
                        Id = Guid.NewGuid(),
                        TrabajadorId = trabajador.Id,
                        Fecha = item.Fecha,
                        HoraEntrada = item.HoraEntrada,
                        HoraSalida = item.HoraSalida,
                        HorasRegulares = item.HorasRegulares,
                        HorasExtras = item.HorasExtras,
                        Tipo = item.Tipo,
                        Observaciones = item.Observaciones,
                        IdExternoPartner = item.IdExternoPartner,
                        Estado = EstadoProcesamiento.Completado,
                        FechaRecepcion = DateTime.UtcNow,
                        CreadoPor = "AsistenciaProcessor"
                    };

                    registros.Add(registro);
                    result.ProcessedCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error procesando registro para {item.DniTrabajador}: {ex.Message}");
                }
            }

            if (registros.Count > 0)
            {
                if (registros.Count > 10000)
                {
                    await _bulkInsertService.BulkInsertAsync(registros, "RegistrosAsistencia");
                }
                else if (registros.Count > 250)
                {
                    await _efBulkService.BulkInsertWithEfAsync(registros, 1000);
                }
                else
                {
                    foreach (var registro in registros)
                        await _unitOfWork.RegistrosAsistencia.AddAsync(registro);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            return result;
        }

        private async Task<Trabajador> CreateTrabajadorAsync(AsistenciaDataDto item, Guid companiaId)
        {
            var trabajador = new Trabajador
            {
                Id = Guid.NewGuid(),
                CompaniaId = companiaId,
                Nombres = item.NombresTrabajador ?? "Sin Nombre",
                Apellidos = item.ApellidosTrabajador ?? "Sin Apellidos",
                Dni = item.DniTrabajador,
                Email = $"{item.DniTrabajador}@empresa.com",
                FechaIngreso = DateTime.UtcNow,
                EsActivo = true,
                IdExternoPartner = item.IdExternoPartner
            };

            await _unitOfWork.Trabajadores.AddAsync(trabajador);
            await _unitOfWork.SaveChangesAsync();

            return trabajador;
        }

        private async Task UpdateTransactionStatusAsync(string transactionId, EstadoProcesamiento estado, int procesados, string? error = null)
        {
            var transaccion = await _unitOfWork.TransaccionesSincronizacion.GetByTransactionIdAsync(transactionId);
            if (transaccion is not null)
            {
                transaccion.Estado = estado;
                transaccion.RegistrosProcesados = procesados;

                if (estado == EstadoProcesamiento.Completado || estado == EstadoProcesamiento.Error)
                {
                    transaccion.FechaFin = DateTime.UtcNow;
                    transaccion.DuracionMs = (long)(transaccion.FechaFin.Value - transaccion.FechaInicio).TotalMilliseconds;
                }

                if (!string.IsNullOrEmpty(error))
                {
                    transaccion.MensajeError = error;
                }

                _unitOfWork.TransaccionesSincronizacion.Update(transaccion);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task UpdateTransactionProgressAsync(string transactionId, int procesados, int errores)
        {
            var transaccion = await _unitOfWork.TransaccionesSincronizacion.GetByTransactionIdAsync(transactionId);
            if (transaccion is not null)
            {
                transaccion.RegistrosProcesados = procesados;
                transaccion.RegistrosErrores = errores;

                _unitOfWork.TransaccionesSincronizacion.Update(transaccion);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}