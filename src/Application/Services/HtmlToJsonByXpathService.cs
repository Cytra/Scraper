using Application.Extensions;
using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


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

    private List<object> GetListItem(HtmlNode document, ExtractRule extractRule)
    {
        var nodes = document
            .SelectNodes(extractRule.Selector);

        var listItems = new List<object>();

        foreach (var node in nodes)
        {
            listItems.Add(GetOutput(node, extractRule.OutputType));
        }

        return listItems;
    }

    private object GetSingleItem(HtmlNode document, ExtractRule extractRules)
    {
        if (extractRules.Output == null)
        {
            var selector = GetSelector(extractRules.Selector);
            var node = document.SelectNodes(selector).FirstOrDefault();
            return GetOutput(node, extractRules.OutputType);
        }

        return HandleNestedObject(document, extractRules);
    }

    private string? GetSelector(string? selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            return null;
        }

        if (selector.StartsWith("//"))
        {
            return selector;
        }

        return $"//{selector}";
    }

    private object HandleNestedObject(
        HtmlNode document,
        ExtractRule extractRules)
    {
        var result = new List<Dictionary<string, object>>();

        var extractRulesObject = GetDictionary(extractRules.Output!);

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

    private object GetOutput(HtmlNode? node, OutputType outputType)
    {
        if (node == null)
        {
            return null;
        }

        return outputType switch
        {
            OutputType.Html => node.InnerHtml,
            OutputType.Text => node.InnerText.Cleanup(),
            _ => node.InnerText.Cleanup()
        };
    }

    private Dictionary<string, ExtractRule>? GetDictionary(object extractRules)
    {
        Dictionary<string, ExtractRule>? extractRulesObject;
        if (extractRules is Dictionary<string, ExtractRule> rules)
        {
            extractRulesObject = rules;
        }
        else
        {
            extractRulesObject = JsonConvert
                .DeserializeObject<Dictionary<string, ExtractRule>>(
                    extractRules.ToString());
        }

        return extractRulesObject;
    }
}