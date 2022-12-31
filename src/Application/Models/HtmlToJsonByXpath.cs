namespace Application.Models;

public class HtmlToJsonByXpath
{
    public required string Url { get; set; }

    public Dictionary<string,object> ExtractRules { get; set; }
}

