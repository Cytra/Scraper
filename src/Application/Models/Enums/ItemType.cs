using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ItemType : byte
{
    None = 0,
    Item = 1,
    List = 2,
    Table = 3,
}