using Application.Interfaces;
using Application.Models;
using System.Text;

namespace Application.Services.Selector;

public class ExplicitSelectorService : ISelectorService<ExplicitExtractRule>
{
    public string GetImplicitInputSelector(ExplicitExtractRule selector)
    {
        (string? element, string? classString, string? id) = selector.Selector;

        if (element == null && string.IsNullOrEmpty(classString) && string.IsNullOrEmpty(id))
        {
            return "//*";
        }

        var xpathBuilder = new StringBuilder("//");

        if (!string.IsNullOrEmpty(element))
        {
            xpathBuilder.Append(element.ToLower());
        }
        else
        {
            xpathBuilder.Append("*");
        }

        if (!string.IsNullOrEmpty(classString))
        {
            xpathBuilder.Append($"[@class='{classString}']");
        }

        if (!string.IsNullOrEmpty(id))
        {
            xpathBuilder.Append($"[@id='{id}']");
        }

        return xpathBuilder.ToString();
    }

    public string GetImplicitOutputSelector(ExplicitExtractRule selector)
    {
        if (selector.Selector.Element is "a" or "link")
        {
            return "href";
        }
        return string.Empty;
    }
}