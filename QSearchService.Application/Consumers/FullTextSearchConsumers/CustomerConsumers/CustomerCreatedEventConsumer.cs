using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.FullTextSearchConsumers.CustomerConsumers;

public class CustomerCreatedEventConsumer : IConsumer<CustomerCreatedEvent>
{
    private readonly ILogger<CustomerCreatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CustomerCreatedEventConsumer(ILogger<CustomerCreatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType}", SearchEntityType.Customer);


        var vectorDoc = new SearchVectorDocument
        {
            EntityId = request.CustomerId,
            EntityType = SearchEntityType.Customer,
            Title = $"{request.FirstName} {request.LastName}",
            Subtitle = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };


        await _dbContext.SearchVectorDocuments.AddAsync(vectorDoc, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document created with Id {Id}", vectorDoc.Id);
    }
}