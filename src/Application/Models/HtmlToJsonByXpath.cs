using Application.Models.Enums;

namespace Application.Models;

public class HtmlToJsonByXpath
{
    public required string Url { get; set; }

    public Dictionary<string, ExtractRule>? ExtractRules { get; set; }
}

public class ExtractRule
{
    /// <summary>
    /// Xpath Selector
    /// </summary>
    public required string Selector { get; set; }

    /// <summary>
    /// What item should it be
    /// </summary>
    public ItemType? ItemType { get; set; }

    /// <summary>
    /// Nested Output 
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    /// If false returns the html
    /// </summary>
    public bool Clean { get; set; } = true;
}

