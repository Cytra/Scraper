using OneOf;

namespace Application.Models;

public class JsonByXpathOneOf : OneOfBase<JsonByXpathImplicit, JsonByXpathExplicit>
{
    JsonByXpathOneOf(OneOf<JsonByXpathImplicit, JsonByXpathExplicit> _) : base(_) { }

    // optionally, define implicit conversions
    // you could also make the constructor public
    public static implicit operator JsonByXpathOneOf(JsonByXpathImplicit _) => new JsonByXpathOneOf(_);
    public static implicit operator JsonByXpathOneOf(JsonByXpathExplicit _) => new JsonByXpathOneOf(_);
}