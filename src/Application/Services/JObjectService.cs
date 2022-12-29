using HtmlAgilityPack;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Application.Services;

public interface IJObjectService
{
    Dictionary<string,object> GetDictionaryFromHtml(string html);
}

//public class HtmlNodeConverter : JsonConverter
//{
//    public override bool CanConvert(Type objectType)
//    {
//        return objectType == typeof(HtmlAgilityPack.HtmlNode);
//    }

//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        throw new NotImplementedException();
//    }

//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        HtmlAgilityPack.HtmlNode node = (HtmlAgilityPack.HtmlNode)value;

//        writer.WriteStartObject();
//        writer.WritePropertyName("Name");
//        serializer.Serialize(writer, node.Name);
//        writer.WritePropertyName("InnerHtml");
//        serializer.Serialize(writer, node.InnerHtml);
//        writer.WriteEndObject();
//    }
//}


public class JObjectService : IJObjectService
{
    public Dictionary<string, object> GetDictionaryFromHtml(string html)
    {
        var result = new Dictionary<string,object>();

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        ParseInnerHtml(htmlDoc.DocumentNode, result, "");

        //JsonSerializerOptions options = new JsonSerializerOptions();
        //options.ReferenceHandler = ReferenceHandler.Preserve;

        //// Serialize the dictionary to a JSON string using the serializer options
        //string json = JsonSerializer.Serialize(result);

        return result;
    }

    static void ParseInnerHtml(HtmlNode node, Dictionary<string, object> htmlToJson, string xpath)
    {
        // Trim the leading and trailing white space characters from the inner HTML
        var innerHtml = node.InnerHtml.Replace("\r\n", "").Trim();

        // Skip adding the element if its InnerHtml is an empty string
        if (string.IsNullOrEmpty(innerHtml))
        {
            return;
        }

        // Create a list to store the child elements
        var children = new List<object>();

        // Recursively parse the inner HTML of each child element
        int i = 1;
        foreach (var child in node.ChildNodes)
        {
            var childNode = new Dictionary<string, object>();
            ParseInnerHtml(child, childNode, xpath + "/" + child.Name + "[" + i + "]");
            if (childNode.Count > 0)
            {
                children.Add(childNode);
            }
            i++;
        }

        if (node.ChildNodes.Count == 0)
        {
            htmlToJson[xpath] = new Dictionary<string, object>()
            {
                { "innerHtml", innerHtml }
            };
        }
        else
        {
            htmlToJson[xpath] = new Dictionary<string, object>()
            {
                //{ "innerHtml", innerHtml },
                { "children", children }
            };
        }
    }
}