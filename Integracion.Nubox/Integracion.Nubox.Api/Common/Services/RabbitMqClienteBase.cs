using Integracion.Nubox.Api.Common.Entities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Integracion.Nubox.Api.Common.Services
{
    public abstract class RabbitMqClienteBase : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IOptions<IntegracionSettings> _integracionSettings;
        private IConnection? _connection;
        private readonly ILogger<RabbitMqClienteBase> _logger;
        protected IChannel? Channel { get; private set; }
        public string Exchange { get; set; } = string.Empty;

        protected RabbitMqClienteBase(ConnectionFactory connectionFactory,
            IOptions<IntegracionSettings> appSettings,
            ILogger<RabbitMqClienteBase> logger)
        {
            _connectionFactory = connectionFactory;
            _integracionSettings = appSettings;
            _logger = logger;

            try
            {
                ConnectToRabbitMq().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during RabbitMQ initialization in constructor");
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                if (Channel != null)
                {
                    Channel.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    Channel.Dispose();
                    Channel = null;
                }

                if (_connection != null)
                {
                    _connection.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    _connection.Dispose();
                    _connection = null;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Cannot dispose RabbitMQ channel or connection");
            }
        }

        private async Task ConnectToRabbitMq()
        {
            try
            {
                var rabbitConfiguration = _integracionSettings.Value.RabbitConfiguration;
                Exchange = rabbitConfiguration.Exchange;

                if (_connection == null || !_connection.IsOpen)
                    _connection = await _connectionFactory.CreateConnectionAsync();

                if (Channel == null || !Channel.IsOpen)
                {
                    Channel = await _connection.CreateChannelAsync();
                    await Channel.ExchangeDeclareAsync(
                        exchange: rabbitConfiguration.Exchange,
                        type: ExchangeType.Direct,
                        durable: true,
                        autoDelete: false);

                    _logger?.LogInformation("RabbitMQ connection and channel established successfully");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al conectar a RabbitMQ");
                throw;
            }
        }
        protected async Task DeclareQueueAsync(string queueName, string routingKey)
        {
            if (Channel == null)
                throw new InvalidOperationException("Channel is not initialized");

            try
            {
                await Channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                await Channel.QueueBindAsync(
                    queue: queueName,
                    exchange: Exchange,
                    routingKey: routingKey);

                _logger?.LogInformation("Queue {QueueName} declared and bound to exchange {Exchange} with routing key {RoutingKey}",
                    queueName, Exchange, routingKey);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error declaring queue {QueueName}", queueName);
                throw;
            }
        }
    }
}