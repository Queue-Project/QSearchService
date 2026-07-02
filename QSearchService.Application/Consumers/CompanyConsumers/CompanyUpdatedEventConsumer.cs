using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;

namespace QSearchService.Application.Consumers.CompanyConsumers;

public class CompanyUpdatedEventConsumer: IConsumer<CompanyUpdatedEvent>
{
    private readonly ILogger<CompanyUpdatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CompanyUpdatedEventConsumer(ILogger<CompanyUpdatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Updating search document for EntityType {EntityType}", SearchEntityType.Company);

        var company = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.CompanyId && s.EntityType == SearchEntityType.Company);

        if (company == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.CompanyId);
            return;
        }

        company.Title = request.CompanyName;
        company.Subtitle = $"{request.PhoneNumber}, {request.EmailAddress}";
        company.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document updated successfully");
        
    }
}