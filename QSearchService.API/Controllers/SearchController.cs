using MediatR;
using Microsoft.AspNetCore.Mvc;
using QSearchService.Application.Requests;
using QSearchService.Application.Responses;
using QSearchService.Application.UseCases.Queries.FullTextSearch;

namespace QSearchService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<ActionResult<PagedResponse<SearchItem>>> FullTextSearch([FromQuery] SearchRequest request)
    {
        var query = new FullTextSearchQuery(request.SearchTerm, request.PageNumber, request.PageSize);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}