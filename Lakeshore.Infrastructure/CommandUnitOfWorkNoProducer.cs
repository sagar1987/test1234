using Lakeshore.Infrastructure.DomainEventsDispatching;
using Lakeshore.Domain;
using Lakeshore.Infrastructure.EntityModelConfiguration;
using MediatR;
using Lakeshore.Kafka.Client.Interfaces;
using System.Reflection;
using Confluent.Kafka;
using System;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;

namespace Lakeshore.Infrastructure;

public class CommandUnitOfWorkNoProducer : ICommandUnitOfWork
{
    private readonly ILogger<CommandUnitOfWorkNoProducer> _logger;
    private readonly SalesAccountContext _context;
    private readonly IMediator _mediator;
    private readonly IDomainEventsAccessor _domainEventsAccessor;

    public CommandUnitOfWorkNoProducer(SalesAccountContext context, 
        IDomainEventsAccessor domainEventsAccessor,
        IMediator mediator,
        ILogger<CommandUnitOfWorkNoProducer> logger
        )
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainEventsAccessor = domainEventsAccessor ?? throw new ArgumentNullException(nameof(domainEventsAccessor));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var domainEvents = _domainEventsAccessor.GetAllDomainEvents();
            _domainEventsAccessor.ClearAllDomainEvents();
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
        }
    }
}
