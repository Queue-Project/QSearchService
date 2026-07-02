using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyConsumers;

public class CompanyCreatedEventConsumer : IConsumer<CompanyCreatedEvent>
{
    private readonly ILogger<CompanyCreatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CompanyCreatedEventConsumer(ILogger<CompanyCreatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType}", SearchEntityType.Company);


        var vectorDoc = new SearchVectorDocument
        {
            EntityId = request.CompanyId,
            EntityType = SearchEntityType.Company,
            Title = request.CompanyName,
            Subtitle = $"{request.PhoneNumber}, {request.EmailAddress}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.SearchVectorDocuments.AddAsync(vectorDoc, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document created with Id {Id}", vectorDoc.Id);
    }
}