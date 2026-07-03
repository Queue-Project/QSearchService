using QSearchService.Application.Responses;

namespace QSearchService.Application.Interfaces;

public interface IElasticSearchService
{
    Task<PagedResponse<SearchItem>> SearchAsync(
        string searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}