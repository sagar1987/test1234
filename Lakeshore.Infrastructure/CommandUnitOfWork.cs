using Lakeshore.Infrastructure.DomainEventsDispatching;
using Lakeshore.Domain;
using Lakeshore.Infrastructure.EntityModelConfiguration;
using MediatR;
using Lakeshore.Kafka.Client.Interfaces;
using System.Reflection;
using Confluent.Kafka;
using System;
using Microsoft.Extensions.Logging;

namespace Lakeshore.Infrastructure;

public class CommandUnitOfWork : ICommandUnitOfWork
{

    private readonly LakeshoreStagingContext _context;
    private readonly IMediator _mediator;
    private readonly IDomainEventsAccessor _domainEventsAccessor;
    private readonly IKafkaProducerClient _kafkaProducerClient;
    private readonly ILogger<CommandUnitOfWork> _logger;

    public CommandUnitOfWork(LakeshoreStagingContext context, 
        IDomainEventsAccessor domainEventsAccessor,
        IMediator mediator,
        IKafkaProducerClient kafkaProducerClient,
        ILogger<CommandUnitOfWork> logger
        )
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainEventsAccessor = domainEventsAccessor ?? throw new ArgumentNullException(nameof(domainEventsAccessor));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _kafkaProducerClient = kafkaProducerClient ?? throw new ArgumentNullException(nameof(kafkaProducerClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        //Commit to Kafka after we have processed the data and updated the database
        var producer = _kafkaProducerClient.Producer;
        var timeout = _kafkaProducerClient.TimeoutInSeconds;
        producer.InitTransactions(System.TimeSpan.FromSeconds(timeout));

        try
        {
            producer.BeginTransaction();

            var domainEvents = _domainEventsAccessor.GetAllDomainEvents();
            _domainEventsAccessor.ClearAllDomainEvents();
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);

            producer.CommitTransaction();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
            producer.AbortTransaction();
        }
    }
}
