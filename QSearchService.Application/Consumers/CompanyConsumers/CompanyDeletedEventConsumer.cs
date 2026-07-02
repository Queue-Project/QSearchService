using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;

namespace QSearchService.Application.Consumers.CompanyConsumers;

public class CompanyDeletedEventConsumer : IConsumer<CompanyDeletedEvent>
{
    private readonly ILogger<CompanyDeletedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CompanyDeletedEventConsumer(ILogger<CompanyDeletedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyDeletedEvent> context)
    {
        var request = context.Message;


        var vectorDoc = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.CompanyId && s.EntityType == SearchEntityType.Company);

        if (vectorDoc == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.CompanyId);
            return;
        }


        _dbContext.SearchVectorDocuments.Remove(vectorDoc);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document deleted successfully");
    }
}