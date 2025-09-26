namespace Integracion.Nubox.Api.Common.Entities
{
    public class Response
    {
        public bool Status { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; } = new { };
    }
}
