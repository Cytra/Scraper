using Application.Models;

namespace Application.Interfaces;

public interface IHtmlParser
{
    Dictionary<string, object?> GetJson(HtmlToJsonByXpath instructions, string html);
}