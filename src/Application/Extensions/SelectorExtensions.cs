namespace Application.Extensions;

internal static class SelectorExtensions
{
    internal static string? GetInputSelector(string? selector)
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

    internal static string GetOutputSelector(string selector)
    {
        return selector[1..];
    }
}