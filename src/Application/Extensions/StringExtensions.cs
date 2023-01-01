namespace Application.Extensions;

internal static class StringExtensions
{
    internal static string Cleanup(this string value)
    {
        return value.Replace("\r\n", "").Trim();
    }
}