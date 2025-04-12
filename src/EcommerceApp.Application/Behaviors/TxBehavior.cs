using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.SeedWorks.Abstractions;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace RookieShop.Persistence;

[DebuggerStepThrough]
public sealed class TxBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
                                    where TRequest : IRequest<TResponse>
                                        where TResponse : notnull
{
    private readonly IPublisher _publisher;
    private readonly IDomainEventContext _eventContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TxBehavior<TRequest, TResponse>> _logger;

    public TxBehavior(
        IPublisher publisher,
        IDomainEventContext eventContext,
        IUnitOfWork unitOfWork,
        ILogger<TxBehavior<TRequest, TResponse>> logger)
    {
        _publisher = publisher;
        _eventContext = eventContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITxRequest) return await next();

        const string behavior = nameof(TxBehavior<TRequest, TResponse>);

        _logger.LogInformation("[{Behavior}] {Request} handled command {CommandName}", behavior, request,
            request.GetType().Name);
        _logger.LogDebug("[{Behavior}] {Request} handled command {CommandName} with {CommandData}", behavior, request,
            request.GetType().Name, request);
        _logger.LogInformation("[{Behavior}]  {Request} begin transaction for command {CommandName}", behavior, request,
            request.GetType().Name);

        await _unitOfWork.BeginTransaction(cancellationToken);

        var response = await next();

        _logger.LogInformation("[{Behavior}] {Request} transaction begin for command {CommandName}",
            behavior, request, request.GetType().Name);

        var domainEvents = _eventContext.GetDomainEvents().ToList();

        _logger.LogInformation(
            "[{Behavior}] {Request} transaction begin for command {CommandName} with {DomainEventsCount} domain events",
            behavior,
            request, request.GetType().Name, domainEvents.Count);

        var tasks = domainEvents.Select(async
            domainEvent =>
        {
            await _publisher.Publish(domainEvent, cancellationToken);

            _logger.LogDebug(
                "[{Behavior}] Published domain event {DomainEventName} with payload {DomainEventContent}",
                behavior, domainEvent.GetType().FullName,
                JsonSerializer.Serialize(domainEvent));
        });

        await Task.WhenAll(tasks);

        await _unitOfWork.CommitTransaction(cancellationToken);

        return response;
    }
}
