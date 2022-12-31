using HtmlAgilityPack;

namespace Application.Services;

public interface IHtmlToJsonService
{
    Dictionary<string,object> GetDictionaryFromHtml(string html);
}

public class HtmlToJsonService : IHtmlToJsonService
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

        if (node.ChildNodes.Count == 0)
        {
            htmlToJson[xpath] = new Dictionary<string, object>()
            {
                { "innerHtml", innerHtml }
            };
            return;
        }

        // Create a list to store the child elements
        var children = new List<object>();

        // Recursively parse the inner HTML of each child element

        for (int i = 0; i < node.ChildNodes.Count; i++)
        {
            var child = node.ChildNodes[i];
            var childNode = new Dictionary<string, object>();
            ParseInnerHtml(child, childNode, xpath + "/" + child.Name + "[" + (i + 1) + "]");
            children.Add(childNode);
        }

        htmlToJson[xpath] = new Dictionary<string, object>()
        {
            //{ "innerHtml", innerHtml },
            { "children", children }
        };
    }
}