using Application.Extensions;
using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IXpathService
{
    Dictionary<string, object?> GetJsonByXpath(HtmlToJsonByXpath instructions, string html);
}

public class XpathService : IXpathService
{
    private readonly ILogger<XpathService> _logger;
    public XpathService(ILogger<XpathService> logger)
    {
        _logger = logger;
    }
    public Dictionary<string, object?> GetJsonByXpath(HtmlToJsonByXpath instructions, string html)
    {
        var result = new Dictionary<string, object?>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        if (instructions.ExtractRules == null)
        {
            return result;
        }

        foreach (var (key, extractRule) in instructions.ExtractRules)
        {
            result.Add(key, GetObjectToAdd(htmlDoc.DocumentNode, extractRule));
        }

        return result;
    }

    private object? GetObjectToAdd(HtmlNode document, ExtractRule extractRule)
    {
        try
        {
            return extractRule.ItemType switch
            {
                ItemType.Item => GetSingleItem(document, extractRule),
                ItemType.List => GetListItem(document, extractRule),
                _ => GetSingleItem(document, extractRule),
            };
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }

        return null;

    }

    private List<string> GetListItem(HtmlNode document, ExtractRule extractRule)
    {
        var selector = GetSelector(extractRule.Selector);
        var nodes = document.SelectNodes(selector);

        var listItems = new List<string>();

        foreach (var node in nodes)
        {
            listItems.Add(GetOutput(node, extractRule.ItemType, extractRule.Selector.Substring(1)));
        }

        return listItems;
    }

    private object GetSingleItem(HtmlNode document, ExtractRule extractRules)
    {
        if (extractRules.Output == null)
        {
            var selector = GetSelector(extractRules.Selector);
            var node = document.SelectNodes(selector).FirstOrDefault();
            return GetOutput(node, extractRules.ItemType , extractRules.Selector.Substring(1));
        }

        return HandleNestedObject(document, extractRules);
    }

    private string? GetSelector(string? selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            return null;
        }

        if (selector.StartsWith("/"))
        {
            return selector;
        }

        if (selector.StartsWith("#"))
        {
            return $"//*[@id='{selector.Substring(1)}']";
        }

        return $"//{selector}";
    }

    private object HandleNestedObject(
        HtmlNode document,
        ExtractRule extractRules)
    {
        var result = new List<Dictionary<string, object>>();

        var extractRulesObject = OutputExtensions.GetDictionary(extractRules.Output!);

        if (extractRulesObject == null)
        {
            return result;
        }

        foreach (var (key, extractRule) in extractRulesObject)
        {
            var nodes = document
                .SelectNodes(extractRule.Selector);

            foreach (var node in nodes)
            {
                var existingDict = result.ElementAtOrDefault(nodes[node]);
                if (existingDict == null)
                {
                    result.Add(new Dictionary<string, object>()
                    {
                        {key, node.InnerText.Cleanup() }
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

    private string GetOutput(HtmlNode? node, ItemType? outputType, string selector)
    {
        if (node == null)
        {
            return null;
        }

        var attribute = node.Attributes.SingleOrDefault(x => x.Name.Contains(selector));
        if (attribute is not null)
        {
            return attribute.Value;
        }

        return outputType switch
        {
            ItemType.Item => node.InnerText.Cleanup(),
            _ => node.InnerText.Cleanup()
        };
    }
}