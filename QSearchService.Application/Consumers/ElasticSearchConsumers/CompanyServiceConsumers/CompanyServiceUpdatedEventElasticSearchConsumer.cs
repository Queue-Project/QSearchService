using BranchService.Contracts.Events.CompanyServiceEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceUpdatedEventElasticSearchConsumer : IConsumer<CompanyServiceUpdatedEvent>
{
    private readonly ILogger<CompanyServiceUpdatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CompanyServiceUpdatedEventElasticSearchConsumer(
        ILogger<CompanyServiceUpdatedEventElasticSearchConsumer> logger, IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CompanyServiceUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");

        var doc = new ElasticSearchIndex
        {
            EntityId = request.CompanyServiceId,
            EntityType = SearchEntityType.CompanyService,
            Title = request.ServiceName,
            Subtitle = request.ServiceDescription,
            SearchText = $"{request.ServiceName} {request.ServiceDescription} "
        };

        await _indexService.UpdateAsync(doc, context.CancellationToken);

        _logger.LogInformation("Search document updated successfully from Elasticsearch");
    }
}