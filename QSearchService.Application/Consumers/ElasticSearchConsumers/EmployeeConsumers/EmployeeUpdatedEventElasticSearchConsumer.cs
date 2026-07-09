using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;

public class EmployeeUpdatedEventElasticSearchConsumer : IConsumer<EmployeeUpdatedEvent>
{
    private readonly ILogger<EmployeeUpdatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public EmployeeUpdatedEventElasticSearchConsumer(ILogger<EmployeeUpdatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<EmployeeUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");

        var doc = new ElasticSearchIndex
        {
            EntityId = request.EmployeeId,
            EntityType = SearchEntityType.Employee,
            Title = $"{request.FirstName} {request.LastName}",
            Subtitle = $"{request.PhoneNumber} {request.Position}",
            SearchText = $"{request.FirstName} {request.LastName} {request.PhoneNumber} {request.Position}"
        };

        await _indexService.UpdateAsync(doc, context.CancellationToken);

        _logger.LogInformation("Search document updated successfully from Elasticsearch");
    }
}