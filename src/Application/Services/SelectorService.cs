using Application.Interfaces;

namespace Application.Services;

public class SelectorService : ISelectorService
{
    public string? GetImplicitInputSelector(string? selector)
    {
        if (string.IsNullOrWhiteSpace(selector))
        {
            return null;
        }

        if (selector.StartsWith("/"))
        {
            return selector;
        }

        if (selector.StartsWith("."))
        {
            return $".//*[contains(@class, '{selector[1..]}')]";
        }

        if (selector.StartsWith("#"))
        {
            return $"//*[@id='{selector[1..]}']";
        }

        return $"//{selector}";
    }

    public string GetImplicitOutputSelector(string selector)
    {
        return selector[1..];
    }
}