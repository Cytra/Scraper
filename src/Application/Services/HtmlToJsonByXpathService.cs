using Application.Extensions;
using Application.Models;
using HtmlAgilityPack;

namespace Application.Services;

public interface IHtmlToJsonByXpathService
{
    Dictionary<string, object> GetJsonByXpath(HtmlToJsonByXpath instructions, string html);
}

public class HtmlToJsonByXpathService : IHtmlToJsonByXpathService
{
    public Dictionary<string, object> GetJsonByXpath(HtmlToJsonByXpath instructions, string html)
    {
        var result = new Dictionary<string, object>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        foreach (var (key, extractRule) in instructions.ExtractRules)
        {
            result.Add(key, GetObjectToAdd(htmlDoc, extractRule));
        }

        return result;
    }

    private object? GetObjectToAdd(HtmlDocument document, ExtractRule extractRule)
    {
        var extractRuleString = extractRule.ToString();
        try
        {
            var nodes = document.DocumentNode.SelectNodes(extractRuleString);

            if (nodes.Count == 1)
            {
                return nodes.Single().InnerText.Cleanup();
            }

            if (nodes.Count > 1)
            {
                var listItems = new List<object>();
                foreach (var node in nodes)
                {
                    listItems.Add(node.InnerText.Cleanup());
                }

                return listItems;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return null;
    }

    private object GetSingleItem(ExtractRule extractRule)
    {
        return null;
    }
}