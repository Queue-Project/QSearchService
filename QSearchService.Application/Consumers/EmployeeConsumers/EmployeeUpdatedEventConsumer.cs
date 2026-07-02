using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.EmployeeConsumers;

public class EmployeeUpdatedEventConsumer: IConsumer<EmployeeUpdatedEvent>
{
    private readonly ILogger<EmployeeUpdatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public EmployeeUpdatedEventConsumer(ILogger<EmployeeUpdatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EmployeeUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Updating search document for EntityType {EntityType}", SearchEntityType.Customer);

        var customer = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.EmployeeId && s.EntityType == SearchEntityType.Employee);

        if (customer == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.EmployeeId);
            return;
        }

        customer.Title = $"{request.FirstName} {request.LastName}";
        customer.Subtitle = request.PhoneNumber;
        customer.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document updated successfully");
        
    }
}