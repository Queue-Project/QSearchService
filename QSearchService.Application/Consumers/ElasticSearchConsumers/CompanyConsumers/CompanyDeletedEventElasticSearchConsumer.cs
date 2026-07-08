using BranchService.Contracts.Events.CompanyEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyConsumers;

public class CompanyDeletedEventElasticSearchConsumer : IConsumer<CompanyDeletedEvent>
{
    private readonly ILogger<CompanyDeletedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CompanyDeletedEventElasticSearchConsumer(ILogger<CompanyDeletedEventElasticSearchConsumer> logger,
        IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CompanyDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        await _indexService.DeleteAsync(request.CompanyId, context.CancellationToken);


        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}