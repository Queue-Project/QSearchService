using BranchService.Contracts.Events.CompanyEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;

public class CompanyCreatedEventElasticSearchConsumer : IConsumer<CompanyCreatedEvent>
{
    private readonly ILogger<CompanyCreatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public CompanyCreatedEventElasticSearchConsumer(ILogger<CompanyCreatedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<CompanyCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType} in Elasticsearch",
            SearchEntityType.Company);


        var doc = new ElasticSearchIndex
        {
            EntityId = request.CompanyId,
            EntityType = SearchEntityType.Company,
            Title = request.CompanyName,
            Subtitle = $"{request.PhoneNumber} {request.EmailAddress}",
            SearchText = $"{request.CompanyName} {request.PhoneNumber} {request.EmailAddress} {request.Address}"
        };

        var response = await _client.IndexAsync(doc, i =>
            i.Index("search-index").Id(request.CompanyId.ToString()), context.CancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError(
                "Failed to index company {CompanyId}: {Reason}",
                request.CompanyId,
                response.ElasticsearchServerError?.Error.Reason);

            throw new Exception(
                $"Failed to index company {request.CompanyId}");
        }

        _logger.LogInformation("Search document created successfully with Id {Id}", request.CompanyId);
    }
}