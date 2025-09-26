using Integracion.Nubox.Api.Common.Entities;
using Integracion.Nubox.Api.Common.Services;
using Integracion.Nubox.Api.Features.Nomina.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Integracion.Nubox.Api.Features.Nomina.Subscribers
{
    public class SincronizarNominaSubscriber : ConsumerBase, IHostedService
    {
        private readonly ILogger<SincronizarNominaSubscriber> _logger;
        private const string QueueName = "SincronizarNominaEvent";
        private const string RoutingKey = "SincronizarNominaEvent";

        public SincronizarNominaSubscriber(ConnectionFactory connectionFactory,
            IOptions<IntegracionSettings> integracionSettings,
            IServiceProvider serviceProvider,
            ILogger<SincronizarNominaSubscriber> logger) :
            base(connectionFactory, integracionSettings, serviceProvider, logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting SincronizarNominaSubscriber");

                await DeclareQueueAsync(QueueName, RoutingKey);
                if (Channel is null)
                {
                    _logger.LogError("Channel is null in SincronizarNominaSubscriber");
                    throw new InvalidOperationException("Channel is not initialized.");
                }
                var consumer = new AsyncEventingBasicConsumer(Channel);
                consumer.ReceivedAsync += OnEventReceived<SincronizarNominaEvent>;

                await Channel!.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("SincronizarNominaSubscriber started and listening on queue: {QueueName}", QueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting SincronizarNominaSubscriber");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping SincronizarNominaSubscriber");
            Dispose();
            return Task.CompletedTask;
        }
    }
}