using Application.Models;
using Application.Models.Enums;
using Application.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Extensions;

public interface IHtmlNodeExtensions
{
    object? GetObjectToAdd(HtmlNode document, ExtractRule extractRule);
}

public class HtmlNodeExtensions : IHtmlNodeExtensions
{
    private readonly ILogger<HtmlParser> _logger;
    public HtmlNodeExtensions(ILogger<HtmlParser> logger)
    {
        _logger = logger;
    }

    public object? GetObjectToAdd(HtmlNode document, ExtractRule extractRule)
    {
        try
        {
            return extractRule.ItemType switch
            {
                ItemType.Item => GetSingleItem(document, extractRule),
                ItemType.List => GetListItem(document, extractRule),
                ItemType.TableArray => GetTableArray(document, extractRule),
                ItemType.TableJson => GetTableJson(document, extractRule),
                _ => GetSingleItem(document, extractRule),
            };
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }

        return null;
    }

    internal object? GetSingleItem(HtmlNode document, ExtractRule extractRules)
    {
        if (extractRules.Output != null) return HandleNestedObject(document, extractRules);

        var nodes = GetNodes(document, extractRules);
        return OutputExtensions.GetOutput(nodes.FirstOrDefault(), extractRules.ItemType, SelectorExtensions.GetOutputSelector(extractRules.Selector));
    }

    internal static List<string> GetListItem(HtmlNode document, ExtractRule extractRule)
    {
        var nodes = GetNodes(document, extractRule);

        var listItems = new List<string>();

        foreach (var node in nodes)
        {
            var outputString = OutputExtensions.GetOutput(node, extractRule.ItemType, SelectorExtensions.GetOutputSelector(extractRule.Selector));
            if (outputString != null)
            {
                listItems.Add(outputString);
            }
        }

        return listItems;
    }

    internal static object? GetTableArray(HtmlNode document, ExtractRule extractRule)
    {
        throw new NotImplementedException();
    }

    internal static object? GetTableJson(HtmlNode document, ExtractRule extractRule)
    {
        throw new NotImplementedException();
    }

    private object HandleNestedObject(
        HtmlNode document,
        ExtractRule extractRules)
    {
        var result = new List<Dictionary<string, object?>>();

        var extractRulesObject = OutputExtensions.GetExtractRules(extractRules.Output!);

        if (extractRulesObject == null)
        {
            return result;
        }
        foreach (var (key, extractRule) in extractRulesObject)
        {
            var nodes = GetNodes(document, extractRule);
            var outputString = GetObjectToAdd(nodes.First(), extractRule);
            if (outputString != null)
            {
                result.Add(new Dictionary<string, object>() { { key, outputString }, });
            }
        }

        return result;
    }

    private static HtmlNodeCollection GetNodes(HtmlNode document, ExtractRule extractRule)
    {
        var selector = SelectorExtensions.GetInputSelector(extractRule.Selector);
        var nodes = document.SelectNodes(selector);
        return nodes;
    }
}