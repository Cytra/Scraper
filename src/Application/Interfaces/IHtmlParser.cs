using Application.Models;

namespace Application.Interfaces;

public interface IHtmlParser<T> where T : ExtractRuleBase
{
    Dictionary<string, object?> GetJson(Dictionary<string, T>? extractRules, string html);
}