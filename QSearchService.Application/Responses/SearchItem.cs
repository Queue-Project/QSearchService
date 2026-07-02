using QSearchService.Domain.Enums;

namespace QSearchService.Application.Responses;

public class SearchItem
{
    public int EntityId { get; set; }
    public SearchEntityType EntityType { get; set; }

    public string Title { get; set; }
    public string? Subtitle { get; set; }
    public float Rank { get; set; }
}