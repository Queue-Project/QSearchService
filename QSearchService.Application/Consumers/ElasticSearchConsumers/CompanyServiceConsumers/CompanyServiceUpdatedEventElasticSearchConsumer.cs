using BranchService.Contracts.Events.CompanyServiceEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceUpdatedEventElasticSearchConsumer : IConsumer<CompanyServiceUpdatedEvent>
{
    private readonly ILogger<CompanyServiceUpdatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CompanyServiceUpdatedEventElasticSearchConsumer(
        ILogger<CompanyServiceUpdatedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CompanyServiceUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");
        var response = await _client.UpdateAsync<ElasticSearchIndex, ElasticSearchIndex>("search-index",
            request.CompanyId.ToString(), u => u.Doc(new ElasticSearchIndex
            {
                EntityId = request.CompanyServiceId,
                EntityType = SearchEntityType.CompanyService,
                Title = request.ServiceName,
                Subtitle = request.ServiceDescription,
                SearchText = $"{request.ServiceName} {request.ServiceDescription}"
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