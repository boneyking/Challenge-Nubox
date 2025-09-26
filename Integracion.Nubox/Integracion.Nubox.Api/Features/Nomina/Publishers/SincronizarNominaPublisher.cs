using Integracion.Nubox.Api.Features.Nomina.Events;
using Integracion.Nubox.Api.Features.Nomina.Requests;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Integracion.Nubox.Api.Features.Nomina.Publishers
{
    public class SincronizarNominaPublisher : ISincronizarNominaPublisher, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<SincronizarNominaPublisher> _logger;

        public SincronizarNominaPublisher(ILogger<SincronizarNominaPublisher> logger,
            ConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public async Task PublishAsync(NominaRequest request)
        {
            IConnection? connection = null;
            IChannel? channel = null;

            try
            {
                var evento = new SincronizarNominaEvent
                {
                    Nomina = request
                };

                connection = await _connectionFactory.CreateConnectionAsync();
                channel = await connection.CreateChannelAsync();
                var json = JsonSerializer.Serialize(evento);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                        exchange: evento.Exchange,
                        routingKey: evento.NombreEvento,
                        body: new ReadOnlyMemory<byte>(body),
                        mandatory: false);

                _logger.LogInformation("Event {EventName} published successfully to exchange {Exchange}",
                    evento.NombreEvento, evento.Exchange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event: {Error}", ex.Message);
                throw;
            }
            finally
            {
                try
                {
                    if (channel != null)
                    {
                        await channel.CloseAsync();
                        channel.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error closing channel");
                }

                try
                {
                    if (connection != null)
                    {
                        await connection.CloseAsync();
                        connection.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error closing connection");
                }
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("SincronizarNominaPublisher disposed");
        }
    }
}