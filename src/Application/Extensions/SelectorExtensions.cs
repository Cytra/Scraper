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

        if (selector.StartsWith("#"))
        {
            return $"//*[@id='{selector.Substring(1)}']";
        }

        return $"//{selector}";
    }

    internal static string GetOutputSelector(string selector)
    {
        return selector.Substring(1);
    }
}