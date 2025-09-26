using MediatR;

namespace Integracion.Nubox.Api.Features.Auth.Requests
{
    public class ReadExcelFileRequest : IRequest<ReadExcelFileResponse>
    {
        public required IFormFile File { get; set; }
    }

    public class ReadExcelFileResponse
    {
        public List<List<string>> Rows { get; set; } = new();
    }
}
