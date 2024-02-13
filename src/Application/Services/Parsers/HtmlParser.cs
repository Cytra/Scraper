using Application.Interfaces;
using Application.Models;
using HtmlAgilityPack;

namespace Application.Services.Parsers;

public class HtmlParser<T> : IHtmlParser<T> where T : ExtractRuleBase
{
    private readonly IJsonExtractorFacade<T> _nodeExtensions;
    public HtmlParser(IJsonExtractorFacade<T> nodeExtensions)
    {
        _nodeExtensions = nodeExtensions;
    }
    public Dictionary<string, object?> GetJson(Dictionary<string, T>? instructions, string html)
    {
        var result = new Dictionary<string, object?>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        if (instructions == null)
        {
            return result;
        }

        foreach (var (key, extractRule) in instructions)
        {
            result.Add(key, _nodeExtensions.GetObjectToAdd(htmlDoc.DocumentNode, extractRule));
        }

        return result;
    }
}