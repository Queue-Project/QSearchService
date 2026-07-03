namespace QSearchService.API;

public class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";

    public string Uri { get; init; } = default!;

    public string DefaultIndex { get; init; } = default!;

    public string? Username { get; init; }

    public string? Password { get; init; }
}