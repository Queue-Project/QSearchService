using MediatR;
using QSearchService.Application.Interfaces;
using QSearchService.Application.Responses;

namespace QSearchService.Application.UseCases.Queries.ElasticSearch;

public class ElasticSearchQueryHandler
    : IRequestHandler<ElasticSearchQuery, PagedResponse<SearchItem>>
{
    private readonly IElasticSearchService _service;

    public ElasticSearchQueryHandler(IElasticSearchService service)
    {
        _service = service;
    }

    public async Task<PagedResponse<SearchItem>> Handle(
        ElasticSearchQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new PagedResponse<SearchItem>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        return await _service.SearchAsync(
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}