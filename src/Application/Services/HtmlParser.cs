using Application.Interfaces;
using Application.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class HtmlParser : IHtmlParser
{
    private readonly ILogger<HtmlParser> _logger;
    private readonly IJsonExtractorFacade _nodeExtensions;
    public HtmlParser(ILogger<HtmlParser> logger, IJsonExtractorFacade nodeExtensions)
    {
        _logger = logger;
        _nodeExtensions = nodeExtensions;
    }
    public Dictionary<string, object?> GetJson(JsonByXpathOneOf instructions, string html)
    {
        var result = new Dictionary<string, object?>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        if (instructions == null)
        {
            return result;
        }

        return instructions.Match(
            implicitInstructions => {
                if (implicitInstructions.ExtractRules == null)
                {
                    return result;
                }

                foreach (var (key, extractRule) in implicitInstructions.ExtractRules)
                {
                    result.Add(key, _nodeExtensions.GetObjectToAdd(htmlDoc.DocumentNode, extractRule));
                }

                return result;
            },
            explicitInstructions => {
                if (explicitInstructions.ExtractRules == null)
                {
                    return result;
                }

                foreach (var (key, extractRule) in explicitInstructions.ExtractRules)
                {
                    result.Add(key, _nodeExtensions.GetObjectToAdd(htmlDoc.DocumentNode, extractRule));
                }

                return result;
            }
        );
    }

}