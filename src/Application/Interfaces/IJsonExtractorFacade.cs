using Application.Models;
using HtmlAgilityPack;

namespace Application.Interfaces;

public interface IJsonExtractorFacade
{
    object? GetObjectToAdd(HtmlNode document, ImplicitExtractRule implicitExtractRule);

    object? GetObjectToAdd(HtmlNode document, ExplicitExtractRule implicitExtractRule);
}