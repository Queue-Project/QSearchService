using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.CustomerEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CustomerConsumers;

public class CustomerDeletedEventElasticSearchConsumer : IConsumer<CustomerDeletedEvent>
{
    private readonly ILogger<CustomerDeletedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CustomerDeletedEventElasticSearchConsumer(ILogger<CustomerDeletedEventElasticSearchConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CustomerDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        var response = await _client.DeleteAsync<ElasticSearchIndex>(request.CustomerId.ToString(),
            d => d.Index("search-index"));

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to delete document with Id {DocumentId} from Elasticsearch: {response.DebugInformation}",
                request.CustomerId, response.DebugInformation);
            throw new Exception(
                $"Failed to delete document with ID {request.CustomerId} from Elasticsearch: {response.DebugInformation}");
        }

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}