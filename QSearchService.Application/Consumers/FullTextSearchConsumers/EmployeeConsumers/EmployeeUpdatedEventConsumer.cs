using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.FullTextSearchConsumers.EmployeeConsumers;

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
        _logger.LogInformation("Updating search document for EntityType {EntityType}", SearchEntityType.Employee);

        var employee = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.EmployeeId && s.EntityType == SearchEntityType.Employee);

        if (employee == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.EmployeeId);
            return;
        }

        employee.Title = $"{request.FirstName} {request.LastName}";
        employee.Subtitle = $"{request.PhoneNumber} {request.Position}";
        employee.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document updated successfully");
        
    }
}