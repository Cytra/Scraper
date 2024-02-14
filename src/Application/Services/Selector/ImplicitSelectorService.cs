using Application.Interfaces;
using Application.Models;

namespace Application.Services.Selector;

public class ImplicitSelectorService : ISelectorService<ImplicitExtractRule>
{
    public string? GetImplicitInputSelector(ImplicitExtractRule selector)
    {
        var selectorString = selector.Selector;
        if (string.IsNullOrWhiteSpace(selectorString))
        {
            return null;
        }

        if (selectorString.StartsWith("/"))
        {
            return selectorString;
        }

        if (selectorString.StartsWith("."))
        {
            return $".//*[contains(@class, '{selectorString[1..]}')]";
        }

        if (selectorString.StartsWith("#"))
        {
            return $"//*[@id='{selectorString[1..]}']";
        }

        return $"//{selectorString}";
    }

    public string GetImplicitOutputSelector(ImplicitExtractRule selector)
    {
        return selector.Selector[1..];
    }
}