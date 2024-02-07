using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum ItemType : byte
{
    None = 0,
    Item = 1,
    List = 2,
    TableJson = 3,
    TableArray = 4,
}