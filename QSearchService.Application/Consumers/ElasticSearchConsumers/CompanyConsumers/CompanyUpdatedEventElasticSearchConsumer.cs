using BranchService.Contracts.Events.CompanyEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyConsumers;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;

public class CompanyUpdatedEventElasticSearchConsumer : IConsumer<CompanyUpdatedEvent>
{
    private readonly ILogger<CompanyUpdatedEventConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CompanyUpdatedEventElasticSearchConsumer(ILogger<CompanyUpdatedEventConsumer> logger,
        ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CompanyUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");
        var response = await _client.UpdateAsync<ElasticSearchIndex, ElasticSearchIndex>("search-index",
            request.CompanyId.ToString(), u => u.Doc(new ElasticSearchIndex
            {
                EntityId = request.CompanyId,
                EntityType = SearchEntityType.Company,
                Title = request.CompanyName,
                Subtitle = $"{request.PhoneNumber} {request.EmailAddress}",
                SearchText = $"{request.CompanyName} {request.PhoneNumber} {request.EmailAddress} {request.Address}"
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