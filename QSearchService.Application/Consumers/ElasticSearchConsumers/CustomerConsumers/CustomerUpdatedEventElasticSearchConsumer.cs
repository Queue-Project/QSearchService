using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;

public class CustomerUpdatedEventElasticSearchConsumer : IConsumer<CustomerUpdatedEvent>
{
    private readonly ILogger<CustomerUpdatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CustomerUpdatedEventElasticSearchConsumer(ILogger<CustomerUpdatedEventElasticSearchConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");
        var response = await _client.UpdateAsync<ElasticSearchIndex, ElasticSearchIndex>("search-index",
            request.CustomerId.ToString(), u => u.Doc(new ElasticSearchIndex
            {
                EntityId = request.CustomerId,
                EntityType = SearchEntityType.Customer,
                Title = $"{request.FirstName} {request.LastName}",
                Subtitle = request.PhoneNumber,
                SearchText = $"{request.FirstName} {request.LastName} {request.PhoneNumber}"
            }).DocAsUpsert(true));


        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to update Elasticsearch: {ErrorReason}",
                response.ElasticsearchServerError?.Error.Reason);
            throw new Exception($"Failed to update Elasticsearch: {response.ElasticsearchServerError?.Error.Reason}");
        }

        _logger.LogInformation("Search document updated successfully from Elasticsearch");
    }
}