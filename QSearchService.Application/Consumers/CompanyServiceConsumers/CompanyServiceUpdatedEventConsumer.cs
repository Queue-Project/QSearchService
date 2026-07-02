using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;

namespace QSearchService.Application.Consumers.CompanyServiceConsumers;

public class CompanyServiceUpdatedEventConsumer : IConsumer<CompanyServiceUpdatedEvent>
{
    private readonly ILogger<CompanyServiceUpdatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CompanyServiceUpdatedEventConsumer(ILogger<CompanyServiceUpdatedEventConsumer> logger,
        ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Updating search document for EntityType {EntityType}", SearchEntityType.Customer);

        var companyService = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.CompanyServiceId && s.EntityType == SearchEntityType.CompanyService);

        if (companyService == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.CompanyId);
            return;
        }

        companyService.Title = request.ServiceName;
        companyService.Subtitle = request.ServiceDescription;
        companyService.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document updated successfully");
    }
}