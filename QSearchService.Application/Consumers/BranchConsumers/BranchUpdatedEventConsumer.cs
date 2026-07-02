using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;

namespace QSearchService.Application.Consumers.BranchConsumers;

public class BranchUpdatedEventConsumer: IConsumer<BranchUpdatedEvent>
{
    private readonly ILogger<BranchUpdatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public BranchUpdatedEventConsumer(ILogger<BranchUpdatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Updating search document for EntityType {EntityType}", SearchEntityType.Branch);

        var branch = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.BranchId && s.EntityType == SearchEntityType.Branch);

        if (branch == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.BranchId);
            return;
        }

        branch.Title = $"{request.BranchId} {request.EmailAddress}";
        branch.Subtitle = request.PhoneNumber;
        branch.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document updated successfully");
        
    }
}