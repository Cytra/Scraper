using System.Reflection;

namespace UnitTests.Helpers;

public static class FileHelpers
{
    public static string GetHtml(string filename)
    {
        var rootFolder = "Data";
        var path = Path.Combine(rootFolder, $"{filename}.html");
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.Combine(assemblyPath, path);
        var rawHtml = File.ReadAllText(fullPath);
        return rawHtml;
    }
}