using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Seeds
{
    public static class CompaniaSeed
    {
        public static void Seed(IntegracionNuboxContext context)
        {
            if (!context.Companias.Any())
            {
                for (var i = 0; i < 100; i++)
                {
                    context.AddRange(new Compania
                    {
                        Id = Guid.NewGuid(),
                        Nombre = $"Compañia {i + 1}",
                    });
                }
                context.SaveChanges();
            }
        }
    }
}
