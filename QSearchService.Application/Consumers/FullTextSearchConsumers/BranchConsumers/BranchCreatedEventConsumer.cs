using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.FullTextSearchConsumers.BranchConsumers;

public class BranchCreatedEventConsumer : IConsumer<BranchCreatedEvent>
{
    private readonly ILogger<BranchCreatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public BranchCreatedEventConsumer(ILogger<BranchCreatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType}", SearchEntityType.Branch);


        var vectorDoc = new SearchVectorDocument
        {
            EntityId = request.BranchId,
            EntityType = SearchEntityType.Branch,
            Title = request.BranchName,
            Subtitle = $"{request.PhoneNumber} {request.EmailAddress}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.SearchVectorDocuments.AddAsync(vectorDoc, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document created with Id {Id}", vectorDoc.Id);
    }
}