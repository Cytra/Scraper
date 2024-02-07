using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Application.Extensions;

internal static class OutputExtensions
{
    internal static string? GetOutput(HtmlNode? node, ItemType? outputType, string selector)
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