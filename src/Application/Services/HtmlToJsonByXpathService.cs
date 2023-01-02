using Application.Extensions;
using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Services;

public interface IHtmlToJsonByXpathService
{
    Dictionary<string, object> GetJsonByXpath(HtmlToJsonByXpath instructions, string html);
}

public class HtmlToJsonByXpathService : IHtmlToJsonByXpathService
{
    private readonly ILogger<HtmlToJsonByXpathService> _logger;
    public HtmlToJsonByXpathService(ILogger<HtmlToJsonByXpathService> logger)
    {
        _logger = logger;
    }
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
        return extractRule.ItemType switch
        {
            ItemType.Item => GetSingleItem(document,extractRule),
            ItemType.List => GetListItem(document, extractRule),
            ItemType.Table => GetTable(document,extractRule),
            _ => GetSingleItem(document, extractRule),
        };
    }

    private object? GetSingleItem(HtmlDocument document, ExtractRule extractRule)
    {
        try
        {
            var selectorString = GetSelectorString(extractRule);
            var node = document.DocumentNode.SelectSingleNode(selectorString);

            return GetOutput(node, document, extractRule);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }
        return null;
    }

    private object? GetListItem(HtmlDocument document, ExtractRule extractRule)
    {
        try
        {
            var selectorString = GetSelectorString(extractRule);
            var nodes = document.DocumentNode.SelectNodes(selectorString);

            var listItems = new List<object>();
            foreach (var node in nodes)
            {
                listItems.Add(GetOutput(node,document, extractRule));
            }

            return listItems;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }
        return null;
    }

    private object GetOutput(HtmlNode node, HtmlDocument document, ExtractRule extractRule)
    {
        return extractRule.OutputType switch
        {
            OutputType.Html => node.InnerHtml,
            OutputType.Text => node.InnerText.Cleanup(),
            //OutputType.Table => GetTable(document,extractRule),
            _ => node.InnerText.Cleanup()
        };
    }

    private object GetTable(HtmlDocument document, ExtractRule extractRules)
    {
        try
        {
            var result = new List<Dictionary<string,object>>();
            var extractRulesObject = JsonConvert
                .DeserializeObject<Dictionary<string, ExtractRule>>(
                extractRules.Output.ToString());

            foreach (var (key, extractRule) in extractRulesObject)
            {
                var selectorString = GetSelectorString(extractRule);
                var nodes = document.DocumentNode
                    .SelectNodes(selectorString);

                foreach (var node in nodes)
                {
                    var existingDict = result.ElementAtOrDefault(nodes[node]);
                    if(existingDict == null)
                    {
                        result.Add(new Dictionary<string, object>()
                        {
                            {key, node.InnerText.Cleanup()}
                        });
                    }
                    else
                    {
                        existingDict.Add(key, node.InnerText.Cleanup());
                    }
                }
            }
            return result;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }
        return null;
    }

    private string? GetSelectorString(ExtractRule extractRule)
    {
        return extractRule.SelectorType switch
        {
            SelectorType.None => extractRule.CssSelector,
            SelectorType.Css => extractRule.CssSelector,
            SelectorType.XPath => extractRule.XpathSelector,
            _ => extractRule.CssSelector
        };
    }
}