using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ElementType
{
    None = 0,
    Body = 1,
    Div = 2,
    Head = 3,
    Anchor = 4,
    Paragraph = 5
}