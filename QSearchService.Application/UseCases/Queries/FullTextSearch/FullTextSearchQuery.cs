using MediatR;
using QSearchService.Application.Responses;

namespace QSearchService.Application.UseCases.Queries.FullTextSearch;

public record FullTextSearchQuery(string SearchTerm, int PageNumber=1, int PageSize=10): IRequest<PagedResponse<SearchItem>>;