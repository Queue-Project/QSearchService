using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QSearchService.Application.Interfaces;
using QSearchService.Application.Responses;

namespace QSearchService.Application.UseCases.Queries.FullTextSearch;

public class FullTextSearchQueryHandler : IRequestHandler<FullTextSearchQuery, PagedResponse<SearchItem>>
{
    private readonly ILogger<FullTextSearchQueryHandler> _logger;
    private readonly ISearchServiceDbContext _context;

    public FullTextSearchQueryHandler(ILogger<FullTextSearchQueryHandler> logger, ISearchServiceDbContext context)
    {
        _logger = logger;
        _context = context;
    }


    public async Task<PagedResponse<SearchItem>> Handle(FullTextSearchQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new PagedResponse<SearchItem>
            {
                Items = [],
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            };
        }

        _logger.LogInformation("Full-text search. SearchTerm: {SearchTerm}, Page: {Page}, Size: {PageSize}",
            request.SearchTerm, request.PageNumber, request.PageSize);


        var query = _context.SearchVectorDocuments
            .AsNoTracking()
            .Where(s => s.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", request.SearchTerm)));


        var totalCount = await query.CountAsync(cancellationToken);

        var results = await query.Select(b => new SearchItem()
            {
                EntityType = b.EntityType,
                EntityId = b.EntityId,
                Title = b.Title,
                Subtitle = b.Subtitle,
                Rank = b.SearchVector.Rank(EF.Functions.PlainToTsQuery("english", request.SearchTerm))
            })
            .OrderByDescending(b => b.Rank)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync(cancellationToken);


        var response = new PagedResponse<SearchItem>
        {
            Items = results,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        _logger.LogInformation("Search completed. Found {Count} matching documents.", totalCount);
        return response;
    }
}