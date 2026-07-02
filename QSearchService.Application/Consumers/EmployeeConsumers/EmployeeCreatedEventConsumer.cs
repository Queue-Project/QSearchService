using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.EmployeeConsumers;

public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public EmployeeCreatedEventConsumer(ILogger<EmployeeCreatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType}", SearchEntityType.Employee);
        

        var vectorDoc = new SearchVectorDocument
        {
            EntityId = request.EmployeeId,
            EntityType = SearchEntityType.Employee,
            Title = $"{request.FirstName} {request.LastName}",
            Subtitle = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.SearchVectorDocuments.AddAsync(vectorDoc, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document created with Id {Id}", vectorDoc.Id);
    }
}