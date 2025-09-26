using Integracion.Nubox.Api.Features.Asistencia.Services.Dto;
using Integracion.Nubox.Api.Infrastructure.Persistence;

namespace Integracion.Nubox.Api.Features.Asistencia.Services
{
    public class AsistenciaValidatorService : IAsistenciaValidatorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AsistenciaValidatorService> _logger;

        public AsistenciaValidatorService(IUnitOfWork unitOfWork, 
            ILogger<AsistenciaValidatorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<ValidationError>> ValidateAsync(List<AsistenciaDataDto> asistenciaData)
        {
            var errors = new List<ValidationError>();

            foreach (var item in asistenciaData)
            {
                var itemErrors = await ValidateSingleAsync(item);
                errors.AddRange(itemErrors);
            }

            return errors;
        }

        public async Task<List<ValidationError>> ValidateSingleAsync(AsistenciaDataDto asistencia)
        {
            var errors = new List<ValidationError>();

            if (string.IsNullOrWhiteSpace(asistencia.DniTrabajador))
            {
                errors.Add(new ValidationError
                {
                    Field = "DniTrabajador",
                    Error = "DNI es requerido",
                    Value = asistencia.DniTrabajador ?? "null"
                });
            }

            if (asistencia.Fecha == default)
            {
                errors.Add(new ValidationError
                {
                    Field = "Fecha",
                    Error = "Fecha es requerida",
                    Value = asistencia.Fecha.ToString()
                });
            }

            if (asistencia.Fecha > DateTime.Now.Date)
            {
                errors.Add(new ValidationError
                {
                    Field = "Fecha",
                    Error = "La fecha no puede ser futura",
                    Value = asistencia.Fecha.ToString("yyyy-MM-dd")
                });
            }

            if (asistencia.HorasRegulares < 0 || asistencia.HorasRegulares > 24)
            {
                errors.Add(new ValidationError
                {
                    Field = "HorasRegulares",
                    Error = "Las horas regulares deben estar entre 0 y 24",
                    Value = asistencia.HorasRegulares.ToString()
                });
            }

            if (asistencia.HorasExtras < 0 || asistencia.HorasExtras > 12)
            {
                errors.Add(new ValidationError
                {
                    Field = "HorasExtras",
                    Error = "Las horas extras deben estar entre 0 y 12",
                    Value = asistencia.HorasExtras.ToString()
                });
            }

            return errors;
        }
    }
}
