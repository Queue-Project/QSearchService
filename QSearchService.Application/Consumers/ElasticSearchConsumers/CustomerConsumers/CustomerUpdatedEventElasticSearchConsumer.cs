using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;

public class CustomerUpdatedEventElasticSearchConsumer : IConsumer<CustomerUpdatedEvent>
{
    private readonly ILogger<CustomerUpdatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CustomerUpdatedEventElasticSearchConsumer(ILogger<CustomerUpdatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");

        var doc = new ElasticSearchIndex
        {
            EntityId = request.CustomerId,
            EntityType = SearchEntityType.Customer,
            Title = $"{request.FirstName} {request.LastName}",
            Subtitle = request.PhoneNumber,
            SearchText = $"{request.FirstName} {request.LastName} {request.PhoneNumber}"
        };

        await _indexService.UpdateAsync(doc, context.CancellationToken);

        _logger.LogInformation("Search document updated successfully from Elasticsearch");
    }
}