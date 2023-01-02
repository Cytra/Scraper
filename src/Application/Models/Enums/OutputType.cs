using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum OutputType : byte
{
    None = 0,
    Text = 1,
    Html = 2,
    Table = 3,
}