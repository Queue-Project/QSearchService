using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.FullTextSearchConsumers.CustomerConsumers;

public class CustomerUpdatedEventConsumer : IConsumer<CustomerUpdatedEvent>
{
    private readonly ILogger<CustomerUpdatedEventConsumer> _logger;
    private readonly ISearchServiceDbContext _dbContext;

    public CustomerUpdatedEventConsumer(ILogger<CustomerUpdatedEventConsumer> logger, ISearchServiceDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Updating search document for EntityType {EntityType}", SearchEntityType.Customer);

        var customer = await _dbContext.SearchVectorDocuments.FirstOrDefaultAsync(s =>
            s.EntityId == request.CustomerId && s.EntityType == SearchEntityType.Customer);

        if (customer == null)
        {
            _logger.LogWarning("Document with EntityId {id} not found", request.CustomerId);
            return;
        }

        customer.Title = $"{request.FirstName} {request.LastName}";
        customer.Subtitle = request.PhoneNumber;
        customer.UpdatedAt = DateTime.UtcNow;


        await _dbContext.SaveChangesAsync(context.CancellationToken);
        _logger.LogInformation("Search document updated successfully");
    }
}