using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;

public class CustomerCreatedEventElasticSearchConsumer : IConsumer<CustomerCreatedEvent>
{
    private readonly ILogger<CustomerCreatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CustomerCreatedEventElasticSearchConsumer(ILogger<CustomerCreatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CustomerCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType} in Elasticsearch",
            SearchEntityType.Customer);


        var doc = new ElasticSearchIndex
        {
            EntityId = request.CustomerId,
            EntityType = SearchEntityType.Customer,
            Title = $"{request.FirstName} {request.LastName}",
            Subtitle = request.PhoneNumber,
            SearchText = $"{request.FirstName} {request.LastName} {request.PhoneNumber}"
        };

        await _indexService.CreateAsync(doc, context.CancellationToken);

        _logger.LogInformation("Search document created successfully with Id {Id}", request.CustomerId);
    }
}