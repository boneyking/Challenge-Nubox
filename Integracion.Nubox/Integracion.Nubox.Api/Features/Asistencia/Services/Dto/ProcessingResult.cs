namespace Integracion.Nubox.Api.Features.Asistencia.Services.Dto
{
    public class ProcessingResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int ProcessedRecords { get; set; }
        public List<string> Errors { get; set; } = [];
        public List<ValidationError> ValidationErrors { get; set; } = [];
        public string? ErrorMessage { get; set; }
        public long ProcessingTimeMs { get; set; }
    }
}
