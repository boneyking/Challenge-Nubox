using Integracion.Nubox.Api.Common.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Integracion.Nubox.Api.Common.Services
{
    public class ConsumerBase : RabbitMqClienteBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerBase> _logger;

        public ConsumerBase(ConnectionFactory connectionFactory,
            IOptions<IntegracionSettings> integracionSettings,
            IServiceProvider serviceProvider,
            ILogger<ConsumerBase> logger)
            : base(connectionFactory, integracionSettings, logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected virtual async Task OnEventReceived<T>(object? sender, BasicDeliverEventArgs eventArgs)
        {
            using var scope = _serviceProvider.CreateScope();
            try
            {
                var body = Encoding.UTF8.GetString(eventArgs.Body.Span);
                _logger.LogInformation("Received message: {Body}", body);

                var message = JsonSerializer.Deserialize<T>(body);

                if (message != null)
                {
                    _logger.LogInformation("Processing message of type {Type}", typeof(T).Name);

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await mediator.Send(message);

                    if (Channel is not null)
                        await Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);

                    _logger.LogInformation("Message processed successfully");
                }
                else
                {
                    _logger.LogWarning("Message deserialization returned null");
                    if (Channel is not null)
                        await Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message from queue");

                try
                {
                    if (Channel != null)
                        await Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                }
                catch (Exception nackEx)
                {
                    _logger.LogError(nackEx, "Error while nacking message");
                }
            }
        }
    }
}