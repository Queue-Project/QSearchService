using NpgsqlTypes;
using QSearchService.Domain.Enums;

namespace QSearchService.Domain.Models;

public class SearchVectorDocument
{
    public int Id { get; set; }

    public int EntityId { get; set; }

    public SearchEntityType EntityType { get; set; }

    public string Title { get; set; }

    public string? Subtitle { get; set; }

    public NpgsqlTsVector SearchVector { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}