using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum SelectorType : byte
{
    None = 0,
    Css = 1,
    XPath = 2,
}