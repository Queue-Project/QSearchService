using QSearchService.Domain.Enums;

namespace QSearchService.Domain.Models;

public class ElasticSearchIndex
{
    public int EntityId { get; set; }
    public SearchEntityType EntityType { get; set; }

    public string Title { get; set; }
    public string Subtitle { get; set; }

    public string SearchText { get; set; }
}