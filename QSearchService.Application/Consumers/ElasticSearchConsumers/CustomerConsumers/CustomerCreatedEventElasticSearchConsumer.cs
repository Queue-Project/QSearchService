using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;

public class CustomerCreatedEventElasticSearchConsumer : IConsumer<CustomerCreatedEvent>
{
    private readonly ILogger<CustomerCreatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CustomerCreatedEventElasticSearchConsumer(ILogger<CustomerCreatedEventElasticSearchConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
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

        var response = await _client.IndexAsync(doc, i =>
            i.Index("search-index").Id(request.CustomerId.ToString()), context.CancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to index customer {CustomerId}: {Reason}",
                request.CustomerId,
                response.ElasticsearchServerError?.Error.Reason);

            throw new Exception(
                $"Failed to index customer {request.CustomerId}");
        }

        _logger.LogInformation("Search document created successfully with Id {Id}", request.CustomerId);
    }
}