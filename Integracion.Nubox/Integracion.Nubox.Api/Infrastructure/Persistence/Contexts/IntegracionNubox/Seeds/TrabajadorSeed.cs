using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Seeds
{
    public static class TrabajadorSeed
    {
        public static void Seed(IntegracionNuboxContext context)
        {
            if (!context.Trabajadores.Any())
            {
                var companias = context.Companias.Take(5).ToList();
                var trabajadores = new List<Trabajador>();

                foreach (var compania in companias)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        trabajadores.Add(new Trabajador
                        {
                            Id = Guid.NewGuid(),
                            CompaniaId = compania.Id,
                            Nombres = $"Trabajador{i}",
                            Apellidos = $"Apellido{i}",
                            Dni = $"123456{i:D2}{companias.IndexOf(compania):D2}",
                            Email = $"trabajador{i}@{compania.Nombre.Replace(" ", "").ToLower()}.com",
                            FechaIngreso = DateTime.UtcNow.AddYears(-1).AddDays(-i * 10),
                            EsActivo = true,
                            IdExternoPartner = $"EXT_{compania.Id}_{i}"
                        });
                    }
                }

                context.Trabajadores.AddRange(trabajadores);
                context.SaveChanges();
            }
        }
    }
}
