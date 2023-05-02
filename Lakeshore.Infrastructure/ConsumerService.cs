using Amazon.Runtime.Internal.Util;
using Confluent.Kafka;
using Lakeshore.Application.ConsumerService;
using Lakeshore.Kafka.Client.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lakeshore.Infrastructure
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger<ConsumerService> _logger;
        private readonly IServiceProvider _services;

        public ConsumerService(ILogger<ConsumerService> logger, IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () => 
            {
                using var scope = _services.CreateScope();

                var kafkaConsumerClient = scope.ServiceProvider.GetRequiredService<IKafkaConsumerClient>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                _logger.LogInformation("Service Started");
                var consumer = kafkaConsumerClient.Consumer;
                consumer.Subscribe(kafkaConsumerClient.Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (consumer == null)
                        {
                            _logger.LogError("Consumer is null. Exiting Service...");
                            return;
                        }

                        var result = consumer.Consume(stoppingToken);
                        _logger.LogInformation("Consuming message '{0}' at topic partition: '{1}'", result.Message.Value, result.TopicPartitionOffset);

                        await mediator.Publish(new KafkaMessageConsumedNotification(result.Message.Value));

                        consumer.Commit(result);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error while consuming message {0}", ex.Error.Reason);
                    }
                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occured: ");
                    }
                }

                _logger.LogInformation("Consumer shutting down...");
                consumer.Close();
            });
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consumer Service Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
