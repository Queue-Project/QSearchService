using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.EmployeeConsumers;

public class EmployeeDeletedEventConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public EmployeeDeletedEventConsumer(ILogger<EmployeeDeletedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        var request = context.Message;


        var vectorDoc = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.EmployeeId && s.EntityType == SearchEntityType.Employee);

        if (vectorDoc == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.EmployeeId);
            return;
        }


        _dbContext.SearchVectorDocuments.Remove(vectorDoc);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document deleted successfully");
    }
}