using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum OutputType : byte
{
    None = 0,
    /// <summary>
    /// Text content of selector, but trimmed of scripts, css, header, footer in order to only keep "content"
    /// </summary>
    Text = 1,
    /// <summary>
    /// HTML content of selector
    /// </summary>
    Html = 2,
    /// <summary>
    /// JSON representation of a <table>
    /// </summary>
    TableJson = 3,
    TableArray = 4,
}