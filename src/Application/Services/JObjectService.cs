using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Net;

namespace Application.Services;

public interface IJObjectService
{
    Dictionary<string,object> GetDictionaryFromHtml(string html);
}

public class JObjectService : IJObjectService
{
    public Dictionary<string, object> GetDictionaryFromHtml(string html)
    {
        var result = new Dictionary<string,object>();

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var body = htmlDoc.DocumentNode.SelectSingleNode("//body");

        if (body == null)
        {
            return result;
        }

        ParseItems(body, result);

        return result;
    }

    private void ParseItems(HtmlNode node, Dictionary<string,object> jObject)
    {
        if (node.HasChildNodes)
        {
            //var innerProperty = new JProperty(childNode.InnerStartIndex.ToString(), childNode.InnerText);
            //var innerJObject = new JObject(innerProperty);
            //if (childNode.HasChildNodes)
            //{
            //    ParseItems(childNode, innerJObject);
            //}

            //var property = new JProperty(childNode.Line.ToString(), childNode.InnerText);

            var innerJObject = new Dictionary<string, object>();
            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.HasChildNodes)
                {
                    ParseItems(childNode, innerJObject);
                }
                else
                {
                    var htmlString = WebUtility.HtmlDecode(childNode
                        .InnerText
                        .Replace("\"", "")
                        .Trim());
                    if (!string.IsNullOrWhiteSpace(htmlString))
                    {
                        innerJObject.TryAdd(childNode.Line.ToString(), htmlString);
                    }
                }
            }

            jObject.Add(node.InnerStartIndex.ToString(), innerJObject);
        }
    }
}