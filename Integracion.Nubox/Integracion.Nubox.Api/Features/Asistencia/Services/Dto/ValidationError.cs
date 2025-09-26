namespace Integracion.Nubox.Api.Features.Asistencia.Services.Dto
{
    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
