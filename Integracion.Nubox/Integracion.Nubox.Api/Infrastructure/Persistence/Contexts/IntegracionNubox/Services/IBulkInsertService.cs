namespace Integracion.Nubox.Api.Infrastructure.Services
{
    public interface IBulkInsertService
    {
        Task BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, CancellationToken cancellationToken = default) where T : class;
    }
}