using ClosedXML.Excel;
using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public class ExcelProcessorService : IExcelProcessorService
    {
        private readonly ILogger<ExcelProcessorService> _logger;

        public ExcelProcessorService(ILogger<ExcelProcessorService> logger)
        {
            _logger = logger;
        }

        public async Task<List<AsistenciaDataDto>> ExtractAsistenciaFromExcelAsync(IFormFile file, Guid companiaId)
        {
            var asistenciaList = new List<AsistenciaDataDto>();

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var workbook = new XLWorkbook(memoryStream);
                var worksheet = workbook.Worksheet(1);
                var rowCount = worksheet.LastRowUsed().RowNumber();

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var asistencia = new AsistenciaDataDto
                        {
                            DniTrabajador = worksheet.Cell(row, 1).GetString().Trim(),
                            NombresTrabajador = worksheet.Cell(row, 2).GetString().Trim(),
                            ApellidosTrabajador = worksheet.Cell(row, 3).GetString().Trim(),
                            Fecha = TryGetDateFromCell(worksheet.Cell(row, 4)),
                            HoraEntrada = worksheet.Cell(row, 5).TryGetValue(out TimeSpan entrada) ? entrada : null,
                            HoraSalida = worksheet.Cell(row, 6).TryGetValue(out TimeSpan salida) ? salida : null,
                            HorasRegulares = worksheet.Cell(row, 7).TryGetValue(out decimal regulares) ? regulares : 0,
                            HorasExtras = worksheet.Cell(row, 8).TryGetValue(out decimal extras) ? extras : 0,
                            Tipo = Enum.TryParse<TipoAsistencia>(worksheet.Cell(row, 9).GetString(), out var tipo) ? tipo : TipoAsistencia.Presente,
                            Observaciones = worksheet.Cell(row, 10).GetString().Trim(),
                            IdExternoPartner = worksheet.Cell(row, 11).GetString().Trim()
                        };

                        if (!string.IsNullOrWhiteSpace(asistencia.DniTrabajador) && asistencia.Fecha != default)
                        {
                            asistenciaList.Add(asistencia);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Error procesando fila {Row}: {Error}", row, ex.Message);
                    }
                }

                _logger.LogInformation("Extraídos {Count} registros de asistencia desde Excel", asistenciaList.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar archivo Excel");
                throw;
            }

            return asistenciaList;
        }

        public async Task<ValidationResult> ValidateExcelFormatAsync(IFormFile file)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
                if (file.Length == 0)
                {
                    result.IsValid = false;
                    result.Errors.Add("El archivo está vacío");
                    return result;
                }

                var allowedExtensions = new[] { ".xlsx", ".xls" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    result.IsValid = false;
                    result.Errors.Add("Formato de archivo no válido. Solo se permiten archivos Excel (.xlsx, .xls)");
                    return result;
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using var workbook = new XLWorkbook(memoryStream);

                if (workbook.Worksheets.Count == 0)
                {
                    result.IsValid = false;
                    result.Errors.Add("El archivo Excel no contiene hojas de cálculo");
                    return result;
                }

                var worksheet = workbook.Worksheet(1);

                if (worksheet.RangeUsed()?.RowCount() < 1)
                {
                    result.IsValid = false;
                    result.Errors.Add("El archivo debe contener al menos una fila de datos además de los headers");
                    return result;
                }

                var expectedHeaders = new[]
                {
                    "DNI", "Nombres", "Apellidos", "Fecha", "Hora Entrada",
                    "Hora Salida", "Horas Regulares", "Horas Extras", "Tipo", "Observaciones"
                };

                for (int col = 1; col <= Math.Min(expectedHeaders.Length, worksheet.LastColumnUsed().ColumnNumber()); col++)
                {
                    var headerValue = worksheet.Cell(1, col).GetString().Trim();
                    if (string.IsNullOrEmpty(headerValue))
                    {
                        result.Warnings.Add($"Header vacío en columna {col}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Error al validar archivo: {ex.Message}");
            }

            return result;
        }

        private DateTime TryGetDateFromCell(IXLCell cell)
        {
            if (cell.TryGetValue(out DateTime fecha))
                return fecha;

            var texto = cell.GetString().Trim();
            if (DateTime.TryParse(texto, out var fechaParseada))
                return fechaParseada;

            if (DateTime.TryParseExact(texto, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var fechaExacta))
                return fechaExacta;

            return default;
        }
    }
}