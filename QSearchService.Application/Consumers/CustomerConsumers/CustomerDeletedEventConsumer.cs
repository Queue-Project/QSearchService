using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.CustomerConsumers;

public class CustomerDeletedEventConsumer: IConsumer<CustomerDeletedEvent>
{
    private readonly ILogger<CustomerDeletedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CustomerDeletedEventConsumer(ILogger<CustomerDeletedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CustomerDeletedEvent> context)
    {
        var request = context.Message;
        

        var vectorDoc = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.CustomerId && s.EntityType == SearchEntityType.Customer);
        
        if ( vectorDoc== null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.CustomerId);
            return;
        }


        _dbContext.SearchVectorDocuments.Remove(vectorDoc);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document deleted successfully");
    }
}