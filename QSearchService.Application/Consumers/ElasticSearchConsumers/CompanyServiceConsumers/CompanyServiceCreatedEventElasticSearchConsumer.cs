using BranchService.Contracts.Events.CompanyServiceEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceCreatedEventElasticSearchConsumer : IConsumer<CompanyServiceCreatedEvent>
{
    private readonly ILogger<CompanyServiceCreatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CompanyServiceCreatedEventElasticSearchConsumer(
        ILogger<CompanyServiceCreatedEventElasticSearchConsumer> logger, IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CompanyServiceCreatedEvent> context)
    {
        var request = context.Message;
        _logger.LogInformation("Creating search document for EntityType {EntityType} in Elasticsearch",
            SearchEntityType.CompanyService);


        var doc = new ElasticSearchIndex
        {
            EntityId = request.CompanyServiceId,
            EntityType = SearchEntityType.CompanyService,
            Title = request.ServiceName,
            Subtitle = request.ServiceDescription,
            SearchText = $"{request.ServiceName} {request.ServiceDescription} "
        };

        await _indexService.CreateAsync(doc, context.CancellationToken);


        _logger.LogInformation("Search document created successfully with Id {Id}", request.CompanyServiceId);
    }
}