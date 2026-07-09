using BranchService.Contracts.Events.CompanyServiceEvents;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Domain.Models;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.CompanyServiceConsumers;

public class CompanyServiceDeletedEventElasticSearchConsumer : IConsumer<CompanyServiceDeletedEvent>
{
    private readonly ILogger<CompanyServiceDeletedEventElasticSearchConsumer> _logger;
    private readonly IElasticSearchIndexService _indexService;

    public CompanyServiceDeletedEventElasticSearchConsumer(
        ILogger<CompanyServiceDeletedEventElasticSearchConsumer> logger, IElasticSearchIndexService indexService)
    {
        _logger = logger;
        _indexService = indexService;
    }

    public async Task Consume(ConsumeContext<CompanyServiceDeletedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Deleting from Elasticsearch");

        await _indexService.DeleteAsync(request.CompanyServiceId, context.CancellationToken);

        _logger.LogInformation("Search document deleted successfully from Elasticsearch");
    }
}