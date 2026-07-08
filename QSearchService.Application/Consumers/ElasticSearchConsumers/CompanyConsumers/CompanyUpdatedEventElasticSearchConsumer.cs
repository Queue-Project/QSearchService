using BranchService.Contracts.Events.CompanyEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Consumers.FullTextSearchConsumers.CompanyConsumers;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;

public class CompanyUpdatedEventElasticSearchConsumer : IConsumer<CompanyUpdatedEvent>
{
    private readonly ILogger<CompanyUpdatedEventConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CompanyUpdatedEventElasticSearchConsumer(ILogger<CompanyUpdatedEventConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CompanyUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");

        var doc = new ElasticSearchIndex
        {
            EntityId = request.CompanyId,
            EntityType = SearchEntityType.Company,
            Title = request.CompanyName,
            Subtitle = $"{request.PhoneNumber} {request.EmailAddress}",
            SearchText = $"{request.CompanyName} {request.PhoneNumber} {request.EmailAddress} {request.Address}"
        };

        await _indexService.UpdateAsync(doc, context.CancellationToken);


        _logger.LogInformation("Search document updated successfully from Elasticsearch");
    }
}