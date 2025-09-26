namespace Integracion.Nubox.Api.Common.Entities
{
    public class IntegracionSettings
    {
        public RabbitMQSettings RabbitConfiguration { get; set; } = new RabbitMQSettings();
    }

    public class RabbitMQSettings
    {
        public string HostRabbitMQ { get; set; } =string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int PortRabbitMQ { get; set; }
        public string Exchange { get; set; } = string.Empty;
    }
}
