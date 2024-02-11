using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Application.Extensions;

internal static class OutputExtensions
{
    internal static string? GetOutput(HtmlNode? node, ItemType? outputType, string selector, bool clean)
    {
        if (node == null)
        {
            return null;
        }

        var attribute = node.Attributes.SingleOrDefault(x => x.Name.Contains(selector));
        if (attribute is not null)
        {
            return attribute.CleanText(clean);
        }

        return outputType switch
        {
            ItemType.Item => node.CleanText(clean),
            _ => node.CleanText(clean)
        };
    }

    private static string? CleanText(this HtmlNode? node, bool clean)
    {
        if (clean)
        {
            return node.InnerText.Cleanup();
        }

        return node.InnerText;
    }

    private static string? CleanText(this HtmlAttribute? node, bool clean)
    {
        if (clean)
        {
            return node.Value.Cleanup();
        }

        return node.Value;
    }

    internal static Dictionary<string, ExtractRule>? GetExtractRules(object extractRules)
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