using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;

namespace QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceDeletedEventConsumer : IConsumer<CompanyServiceDeletedEvent>
{
    private readonly ILogger<CompanyServiceDeletedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CompanyServiceDeletedEventConsumer(ILogger<CompanyServiceDeletedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceDeletedEvent> context)
    {
        var request = context.Message;


        var vectorDoc = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.CompanyServiceId && s.EntityType == SearchEntityType.CompanyService);

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