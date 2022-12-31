using Application.Models;
using HtmlAgilityPack;

namespace Application.Services;

public interface IHtmlToJsonByXpathService
{
    Dictionary<string, object> GetJsonByXpath(HtmlToJsonByXpath instructions, string html);
}

public class HtmlToJsonByXpathService : IHtmlToJsonByXpathService
{
    public HtmlToJsonByXpathService()
    {
        
    }

    public Dictionary<string, object> GetJsonByXpath(HtmlToJsonByXpath instructions, string html)
    {
        var result = new Dictionary<string, object>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        foreach (var extractRule in instructions.ExtractRules)
        {
            //var test = htmlDoc.DocumentNode.
            var extractRuleString = extractRule.Value.ToString();
            var text = htmlDoc.DocumentNode.SelectSingleNode(extractRuleString);
        }

        return result;
    }
}