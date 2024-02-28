using Application.Models;

namespace Scraper.Models;

public class JsonExplicitRequest
{
    public required string Url { get; set; }

    public Dictionary<string, ExplicitExtractRule>? ExtractRules { get; set; }

    public int? WaitTime { get; set; }
}