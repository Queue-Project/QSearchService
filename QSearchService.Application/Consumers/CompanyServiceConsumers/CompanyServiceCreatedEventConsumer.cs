using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.CompanyServiceConsumers;

public class CompanyServiceCreatedEventConsumer : IConsumer<CompanyServiceCreatedEvent>
{
    private readonly ILogger<CompanyServiceCreatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CompanyServiceCreatedEventConsumer(ILogger<CompanyServiceCreatedEventConsumer> logger,
        ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType}", SearchEntityType.CompanyService);


        var vectorDoc = new SearchVectorDocument
        {
            EntityId = request.CompanyServiceId,
            EntityType = SearchEntityType.CompanyService,
            Title = request.ServiceName,
            Subtitle = request.ServiceDescription,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.SearchVectorDocuments.AddAsync(vectorDoc, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document created with Id {Id}", vectorDoc.Id);
    }
}