using MediatR;
using QSearchService.Application.Responses;

namespace QSearchService.Application.UseCases.Queries.ElasticSearch;

public record ElasticSearchQuery(string SearchTerm, int PageNumber=1, int PageSize=10): IRequest<PagedResponse<SearchItem>>;