using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using Application.Models.Enums;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Services;

public class JsonExtractorFacade<T> : IJsonExtractorFacade<T> where T : ExtractRuleBase
{
    private readonly ISelectorService<T> _selectorService;
    private readonly ILogger<JsonExtractorFacade<T>> _logger;
    public JsonExtractorFacade(
        ISelectorService<T> selectorService, ILogger<JsonExtractorFacade<T>> logger)
    {
        _selectorService = selectorService;
        _logger = logger;
    }

    public object? GetObjectToAdd(HtmlNode document, T extractRule)
    {
        try
        {
            return extractRule.ItemType switch
            {
                ItemType.Item => GetSingleItem(document, extractRule),
                ItemType.List => GetListItem(document, extractRule),
                ItemType.TableJson => GetTableJson(document, extractRule),
                ItemType.TableArray => GetTableArray(document, extractRule),
                _ => GetSingleItem(document, extractRule),
            };
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
        }

        return null;
    }

    internal object? GetSingleItem(HtmlNode document, T implicitExtractRules)
    {
        if (implicitExtractRules.Output != null) return HandleNestedObject(document, implicitExtractRules);

        var nodes = GetNodes(document, implicitExtractRules);
        return OutputExtensions.GetOutput(
            nodes.FirstOrDefault(),
            implicitExtractRules.ItemType,
            _selectorService.GetImplicitOutputSelector(implicitExtractRules),
            implicitExtractRules.Clean
            );
    }

    internal List<object> GetListItem(HtmlNode document, T implicitExtractRule)
    {
        var nodes = GetNodes(document, implicitExtractRule);

        var listItems = new List<object>();

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
                _selectorService.GetImplicitOutputSelector(implicitExtractRule),
                implicitExtractRule.Clean);
            if (outputString != null)
            {
                listItems.Add(outputString);
            }
        }

        return listItems;
    }

    internal object GetTableJson(HtmlNode document, T implicitExtractRule)
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

    internal object GetTableArray(HtmlNode document, T implicitExtractRule)
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
        T implicitExtractRules)
    {
        var result = new List<Dictionary<string, object?>>();

        var extractRulesObject = GetExtractRules(implicitExtractRules.Output!);

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

    private HtmlNodeCollection GetNodes(HtmlNode document, T implicitExtractRule)
    {
        var selector = _selectorService.GetImplicitInputSelector(implicitExtractRule);
        var nodes = document.SelectNodes(selector);
        return nodes;
    }

    internal static Dictionary<string, T>? GetExtractRules(object extractRules)
    {
        Dictionary<string, T>? extractRulesObject;
        if (extractRules is Dictionary<string, T> rules)
        {
            extractRulesObject = rules;
        }
        else
        {
            extractRulesObject = JsonConvert
                .DeserializeObject<Dictionary<string, T>>(
                    extractRules.ToString());
        }

        return extractRulesObject;
    }
}