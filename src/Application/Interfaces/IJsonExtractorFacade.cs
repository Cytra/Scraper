using Application.Models;
using HtmlAgilityPack;

namespace Application.Interfaces;

public interface IJsonExtractorFacade<T> where T : ExtractRuleBase
{
    object? GetObjectToAdd(HtmlNode document, T extractRule);
}