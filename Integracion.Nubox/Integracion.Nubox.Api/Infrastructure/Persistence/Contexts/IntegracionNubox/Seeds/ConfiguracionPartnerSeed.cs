using Integracion.Nubox.Api.Common.Entities.Enums;
using Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Models;

namespace Integracion.Nubox.Api.Infrastructure.Persistence.Contexts.IntegracionNubox.Seeds
{
    public static class ConfiguracionPartnerSeed
    {
        public static void Seed(IntegracionNuboxContext context)
        {
            if (!context.ConfiguracionesPartner.Any())
            {
                var companias = context.Companias.Take(5).ToList();
                var configuraciones = new List<ConfiguracionPartner>();

                foreach (var compania in companias)
                {
                    configuraciones.Add(new ConfiguracionPartner
                    {
                        Id = Guid.NewGuid(),
                        CompaniaId = compania.Id,
                        NombrePartner = "PartnerAsistencia Demo",
                        BaseUrl = "https://demo-partner-api.com",
                        ApiKey = $"api_key_demo_{compania.Id}",
                        ClientId = $"client_{compania.Id}",
                        ClientSecret = "demo_secret_123",
                        EsActivo = true,
                        TipoIntegracionPreferido = TipoIntegracion.API,
                        IntervaloSincronizacion = TimeSpan.FromHours(4),
                        SincronizacionAutomatica = true,
                        NotificarErrores = true,
                        EmailNotificaciones = $"admin@{compania.Nombre.Replace(" ", "").ToLower()}.com",
                        TimeoutSegundos = 60,
                        MaxReintentos = 3,
                        TamañoLoteMaximo = 500
                    });
                }

                context.ConfiguracionesPartner.AddRange(configuraciones);
                context.SaveChanges();
            }
        }
    }
}
