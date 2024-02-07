using Application.Models;
using Newtonsoft.Json;

namespace Application.Extensions;

public static class OutputExtensions
{
    public static Dictionary<string, ExtractRule>? GetDictionary(object extractRules)
    {
        Dictionary<string, ExtractRule>? extractRulesObject;
        if (extractRules is Dictionary<string, ExtractRule> rules)
        {
            extractRulesObject = rules;
        }
        else
        {
            extractRulesObject = JsonConvert
                .DeserializeObject<Dictionary<string, ExtractRule>>(
                    extractRules.ToString());
        }

        return extractRulesObject;
    }
}