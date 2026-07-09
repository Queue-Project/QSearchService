using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;

public class EmployeeDeletedEventElasticSearchConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;
    
    
    public EmployeeDeletedEventElasticSearchConsumer(ILogger<EmployeeDeletedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        var response = await _client.DeleteAsync<ElasticSearchIndex>(request.EmployeeId.ToString(),
            d => d.Index("search-index"));

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to delete document with Id {DocumentId} from Elasticsearch: {response.DebugInformation}",
                request.EmployeeId, response.DebugInformation);
            throw new Exception(
                $"Failed to delete document with ID {request.EmployeeId} from Elasticsearch: {response.DebugInformation}");
        }

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}