namespace Application.Models;

public class JsonByXpathExplicit
{
    public required string Url { get; set; }

    public Dictionary<string, ExplicitExtractRule>? ExtractRules { get; set; }
}