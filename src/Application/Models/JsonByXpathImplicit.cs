namespace Application.Models;

public class JsonByXpathImplicit
{
    public required string Url { get; set; }

    public Dictionary<string, ImplicitExtractRule>? ExtractRules { get; set; }
}