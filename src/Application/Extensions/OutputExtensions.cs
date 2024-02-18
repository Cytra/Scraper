using Application.Models.Enums;
using HtmlAgilityPack;

namespace Application.Extensions;

internal static class OutputExtensions
{
    internal static string? GetOutput(HtmlNode? node, ItemType? outputType, string selector, bool clean)
    {
        if (node == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(selector))
        {
            var attribute = node.Attributes.FirstOrDefault(x => x.Name.Contains(selector));
            if (attribute is not null)
            {
                return attribute.CleanText(clean);
            }
        }
        
        return outputType switch
        {
            ItemType.Item => node.CleanText(clean),
            _ => node.CleanText(clean)
        };
    }

    private static string? CleanText(this HtmlNode? node, bool clean)
    {
        return clean ? node.InnerText.Cleanup() : node.InnerText;
    }

    private static string? CleanText(this HtmlAttribute? node, bool clean)
    {
        return clean ? node.Value.Cleanup() : node.Value;
    }
}