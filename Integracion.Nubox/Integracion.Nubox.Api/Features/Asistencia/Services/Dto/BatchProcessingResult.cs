namespace Integracion.Nubox.Api.Features.Asistencia.Services.Dto
{
    public class BatchProcessingResult
    {
        public int ProcessedCount { get; set; }
        public List<string> Errors { get; set; } = [];
    }
}
