using Application.Models.Enums;

namespace Application.Models;

public class HtmlToJsonByXpath
{
    public required string Url { get; set; }

    public Dictionary<string, ExtractRule> ExtractRules { get; set; }
}

public class ExtractRule
{
    public string XpathSelector { get; set; }

    public string CssSelector { get; set; }

    public ItemType Type { get; set; }
}

