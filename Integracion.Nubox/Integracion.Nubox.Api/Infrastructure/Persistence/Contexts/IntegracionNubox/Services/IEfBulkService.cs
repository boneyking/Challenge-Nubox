namespace Integracion.Nubox.Api.Infrastructure.Services
{
    public interface IEfBulkService
    {
        Task BulkInsertWithEfAsync<T>(IEnumerable<T> entities, int batchSize = 1000, CancellationToken cancellationToken = default) where T : class;
    }
}