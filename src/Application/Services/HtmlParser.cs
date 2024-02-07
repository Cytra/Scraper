using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class HtmlParser : IHtmlParser
{
    private readonly ILogger<HtmlParser> _logger;
    private readonly IHtmlNodeExtensions _nodeExtensions;
    public HtmlParser(ILogger<HtmlParser> logger, IHtmlNodeExtensions nodeExtensions)
    {
        _logger = logger;
        _nodeExtensions = nodeExtensions;
    }
    public Dictionary<string, object?> GetJson(HtmlToJsonByXpath instructions, string html)
    {
        var result = new Dictionary<string, object?>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        if (instructions.ExtractRules == null)
        {
            return result;
        }

        foreach (var (key, extractRule) in instructions.ExtractRules)
        {
            result.Add(key, _nodeExtensions.GetObjectToAdd(htmlDoc.DocumentNode, extractRule));
        }

        return result;
    }
}