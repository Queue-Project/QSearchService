using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;

namespace QSearchService.Application.Consumers.BranchConsumers;

public class BranchDeletedEventConsumer : IConsumer<BranchDeletedEvent>
{
    private readonly ILogger<BranchDeletedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public BranchDeletedEventConsumer(ILogger<BranchDeletedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchDeletedEvent> context)
    {
        var request = context.Message;


        var vectorDoc = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.BranchId && s.EntityType == SearchEntityType.Branch);

        if (vectorDoc == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.BranchId);
            return;
        }


        _dbContext.SearchVectorDocuments.Remove(vectorDoc);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document deleted successfully");
    }
}