using Application.Models;
using OneOf;

namespace Application.Interfaces;

public interface IHtmlParser
{
    Dictionary<string, object?> GetJson(JsonByXpathOneOf instructions, string html);
}