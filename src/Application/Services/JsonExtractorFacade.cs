using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class JsonExtractorFacade : IJsonExtractorFacade
{
    private readonly ILogger<HtmlParser> _logger;
    private readonly ISelectorService _selectorService;
    public JsonExtractorFacade(
        ILogger<HtmlParser> logger,
        ISelectorService selectorService)
    {
        _logger = logger;
        _selectorService = selectorService;
    }

    public object? GetObjectToAdd(HtmlNode document, ImplicitExtractRule implicitExtractRule)
    {
        try
        {
            return implicitExtractRule.ItemType switch
            {
                ItemType.Item => GetSingleItem(document, implicitExtractRule),
                ItemType.List => GetListItem(document, implicitExtractRule),
                ItemType.TableJson => GetTableJson(document, implicitExtractRule),
                ItemType.TableArray => GetTableArray(document, implicitExtractRule),
                _ => GetSingleItem(document, implicitExtractRule),
            };
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }

        return null;
    }

    public object? GetObjectToAdd(HtmlNode document, ExplicitExtractRule implicitExtractRule)
    {
        throw new NotImplementedException();
    }

    internal object? GetSingleItem(HtmlNode document, ImplicitExtractRule implicitExtractRules)
    {
        if (implicitExtractRules.Output != null) return HandleNestedObject(document, implicitExtractRules);

        var nodes = GetNodes(document, implicitExtractRules);
        return OutputExtensions.GetOutput(
            nodes.FirstOrDefault(),
            implicitExtractRules.ItemType,
            _selectorService.GetImplicitOutputSelector(implicitExtractRules.Selector),
            implicitExtractRules.Clean
            );
    }

    internal List<object> GetListItem(HtmlNode document, ImplicitExtractRule implicitExtractRule)
    {

        var nodes = GetNodes(document, implicitExtractRule);

        var listItems = new List<object>();

        if (nodes == null)
        {
            return listItems;
        }

        foreach (var node in nodes)
        {
            if (implicitExtractRule.Output != null)
            {
                var nestedObject = HandleNestedObject(document, implicitExtractRule);
                if (nestedObject != null)
                {
                    listItems.Add(nestedObject);
                }
                continue;
            }

            var outputString = OutputExtensions.GetOutput(
                node,
                implicitExtractRule.ItemType,
                _selectorService.GetImplicitOutputSelector(implicitExtractRule.Selector),
                implicitExtractRule.Clean);
            if (outputString != null)
            {
                listItems.Add(outputString);
            }
        }

        return listItems;
    }

    internal object GetTableJson(HtmlNode document, ImplicitExtractRule implicitExtractRule)
    {
        var tableJson = new List<Dictionary<string, string>>();
        var node = GetNodes(document, implicitExtractRule).FirstOrDefault();

        var headers = node.SelectNodes(".//thead//th").Select(th => th.InnerText.Trim()).ToArray();

        var rows = node.SelectNodes(".//tbody//tr");
        foreach (var row in rows)
        {
            var rowData = row.SelectNodes("td").Select(td => td.InnerText.Trim()).ToArray();

            var item = new Dictionary<string, string>();
            for (int i = 0; i < headers.Length; i++)
            {
                item[headers[i]] = rowData[i];
            }

            tableJson.Add(item);
        }
        return tableJson;
    }

    internal object GetTableArray(HtmlNode document, ImplicitExtractRule implicitExtractRule)
    {
        var tableArray = new List<List<string>>();
        var node = GetNodes(document, implicitExtractRule).FirstOrDefault();

        if (node != null)
        {
            var rows = node.SelectNodes(".//tbody//tr");
            if (rows is not null && rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    var rowData = row.SelectNodes("td").Select(td => td.InnerText.Trim()).ToList();
                    tableArray.Add(rowData);
                }
            }
        }

        return tableArray;
    }

    private object HandleNestedObject(
        HtmlNode document,
        ImplicitExtractRule implicitExtractRules)
    {
        var result = new List<Dictionary<string, object?>>();

        var extractRulesObject = OutputExtensions.GetExtractRules(implicitExtractRules.Output!);

        if (extractRulesObject == null)
        {
            return result;
        }
        foreach (var (key, extractRule) in extractRulesObject)
        {
            var nodes = GetNodes(document, extractRule);
            var outputString = GetObjectToAdd(nodes.First(), extractRule);
            if (outputString != null)
            {
                result.Add(new Dictionary<string, object>() { { key, outputString }, });
            }
        }

        return result;
    }

    private HtmlNodeCollection GetNodes(HtmlNode document, ImplicitExtractRule implicitExtractRule)
    {
        var selector = _selectorService.GetImplicitInputSelector(implicitExtractRule.Selector);
        var nodes = document.SelectNodes(selector);
        return nodes;
    }
}