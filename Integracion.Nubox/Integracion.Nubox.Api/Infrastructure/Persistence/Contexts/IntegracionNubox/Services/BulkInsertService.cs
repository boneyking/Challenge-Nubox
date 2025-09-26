using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;

namespace Integracion.Nubox.Api.Infrastructure.Services
{
    public class BulkInsertService : IBulkInsertService
    {
        private readonly string _connectionString;
        private readonly ILogger<BulkInsertService> _logger;

        public BulkInsertService(IConfiguration configuration, 
            ILogger<BulkInsertService> logger)
        {
            _connectionString = configuration.GetConnectionString("IntegracionNuboxConnection")
                ?? throw new InvalidOperationException("Connection string not found");
            _logger = logger;
        }

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, string tableName, CancellationToken cancellationToken = default) where T : class
        {
            if (!entities.Any())
            {
                _logger.LogWarning("No entities to insert");
                return;
            }

            _logger.LogInformation("Starting bulk insert of {Count} entities to table {TableName}", entities.Count(), tableName);

            var dataTable = ConvertToDataTable(entities);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();
            try
            {
                using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)
                {
                    DestinationTableName = tableName,
                    BatchSize = 10000,
                    BulkCopyTimeout = 300
                };

                foreach (DataColumn column in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Bulk insert completed successfully. {Count} records inserted", entities.Count());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error during bulk insert");
                throw;
            }
        }
        private DataTable ConvertToDataTable<T>(IEnumerable<T> entities)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
                .ToArray();

            foreach (var property in properties)
            {
                var columnType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                dataTable.Columns.Add(property.Name, columnType);
            }

            foreach (var entity in entities)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    var value = property.GetValue(entity);
                    row[property.Name] = value ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid) ||
                   Nullable.GetUnderlyingType(type) != null;
        }
    }
}