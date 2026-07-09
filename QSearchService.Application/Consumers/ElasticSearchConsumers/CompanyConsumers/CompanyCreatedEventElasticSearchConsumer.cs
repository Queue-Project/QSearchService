using BranchService.Contracts.Events.CompanyEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;

public class CompanyCreatedEventElasticSearchConsumer : IConsumer<CompanyCreatedEvent>
{
    private readonly ILogger<CompanyCreatedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CompanyCreatedEventElasticSearchConsumer(ILogger<CompanyCreatedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
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

        await _indexService.CreateAsync(doc, context.CancellationToken);


        _logger.LogInformation("Search document created successfully with Id {Id}", request.CompanyId);
    }
}