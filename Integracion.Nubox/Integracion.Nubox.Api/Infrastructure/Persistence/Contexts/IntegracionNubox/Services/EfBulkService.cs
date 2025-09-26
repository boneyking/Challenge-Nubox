using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox;

namespace Integracion.Nubox.Api.Infrastructure.Services
{
    public class EfBulkService : IEfBulkService
    {
        private readonly IntegracionNuboxContext _context;
        private readonly ILogger<EfBulkService> _logger;

        public EfBulkService(IntegracionNuboxContext context, ILogger<EfBulkService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task BulkInsertWithEfAsync<T>(IEnumerable<T> entities, int batchSize = 1000, CancellationToken cancellationToken = default) where T : class
        {
            var entitiesList = entities.ToList();
            if (!entitiesList.Any())
            {
                _logger.LogWarning("No entities to insert");
                return;
            }

            _logger.LogInformation("Starting EF bulk insert of {Count} entities with batch size {BatchSize}", entitiesList.Count, batchSize);

            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                var batches = entitiesList.Chunk(batchSize);
                var batchNumber = 1;

                foreach (var batch in batches)
                {
                    _logger.LogInformation("Processing batch {BatchNumber}", batchNumber);

                    await _context.Set<T>().AddRangeAsync(batch, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    _context.ChangeTracker.Clear();

                    batchNumber++;
                }

                _logger.LogInformation("EF bulk insert completed successfully. {Count} records inserted", entitiesList.Count);
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }
    }
}