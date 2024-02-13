namespace Application.Models;

public class ImplicitExtractRule : ExtractRuleBase
{
    /// <summary>
    /// Xpath Selector
    /// </summary>
    public required string Selector { get; set; }
}