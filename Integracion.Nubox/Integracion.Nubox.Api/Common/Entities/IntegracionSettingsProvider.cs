using Microsoft.Extensions.Options;

namespace Integracion.Nubox.Api.Common.Entities
{
    public interface IIntegracionSettingsProvider
    {
        IntegracionSettings GetSettings();
    }
    public class IntegracionSettingsProvider : IIntegracionSettingsProvider
    {
        private readonly IOptions<IntegracionSettings> _settings;
        public IntegracionSettingsProvider(IOptions<IntegracionSettings> settings) => _settings = settings;
        public IntegracionSettings GetSettings() => _settings.Value;
    }
}
