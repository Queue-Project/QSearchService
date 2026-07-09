using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.Extensions.Logging;
using QSearchService.Domain.Enums;
using QSearchService.Domain.Models;
using QUserService.Contracts.Events.EmployeeEvent;

namespace QSearchService.Application.Consumers.ElasticSearchConsumers.EmployeeConsumers;

public class EmployeeUpdatedEventElasticSearchConsumer: IConsumer<EmployeeUpdatedEvent>
{
    private readonly ILogger<EmployeeUpdatedEventElasticSearchConsumer> _logger;
    private readonly ElasticsearchClient _client;

    public EmployeeUpdatedEventElasticSearchConsumer(ILogger<EmployeeUpdatedEventElasticSearchConsumer> logger, ElasticsearchClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<EmployeeUpdatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Updating document from Elasticsearch");
        var response = await _client.UpdateAsync<ElasticSearchIndex, ElasticSearchIndex>("search-index",
            request.EmployeeId.ToString(), u => u.Doc(new ElasticSearchIndex
            {
                EntityId = request.EmployeeId,
                EntityType = SearchEntityType.Employee,
                Title = $"{request.FirstName} {request.LastName}",
                Subtitle = $"{request.PhoneNumber} {request.Position}",
                SearchText = $"{request.FirstName} {request.LastName} {request.PhoneNumber} {request.Position}"
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