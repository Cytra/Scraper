using System.Text.Json;
using Application.Models;
using Application.Models.Enums;
using Application.Services;
using AutoFixture;
using FluentAssertions;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Services;

public class GetJsonScrapingBeeTests
{
    private readonly IFixture _fixture;
    private const string Html = "ScrapingBeeTable";

    public GetJsonScrapingBeeTests()
    {
        _fixture = RealClassFixture.Create();
    }

    [Fact]
    public void GetTitleSubtitle()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "h1"
                    }
                },
                {
                    "subtitle", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "#subtitle"
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        result.Count.Should().Be(2);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("title");
        keys[1].Should().Be("subtitle");
        values[0].Should().Be("Documentation - Data Extraction");
        values[1].Should().Be("Extract data with CSS or XPATH selectors");
    }

    [Fact]
    public void GetTitle_DifferentTypes()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "#title"
                    }
                },
                {
                    "title2", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "//*[@id='title']" //*[@id='yourNodeId']
                    }
                },
                {
                    "title3", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "//h1[@id='title']"
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        result.Count.Should().Be(3);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("title");
        keys[1].Should().Be("title2");
        keys[2].Should().Be("title3");
        values[0].Should().Be("Documentation - Data Extraction");
        values[1].Should().Be("Documentation - Data Extraction");
        values[2].Should().Be("Documentation - Data Extraction");
    }

    [Fact]
    public void GetFirstRef()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "link", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "@href"
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        result.Count.Should().Be(1);
        result.FirstOrDefault().Key.Should().Be("link");
        result.FirstOrDefault().Value.Should().Be("https://www.scrapingbee.com/index.xml");
    }

    [Fact]
    public void GetAllRefs()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "link", new ExtractRule
                    {
                        ItemType = ItemType.List,
                        Selector = "@href"
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        result.Count.Should().Be(1);
        result.FirstOrDefault().Key.Should().Be("link");
        var links = result.FirstOrDefault().Value as List<string>;
        links!.Count.Should().Be(85);
        links.FirstOrDefault().Should().Be("https://www.scrapingbee.com/index.xml");
    }

    [Fact]
    public void GetTableJson()
    {
        //{
        //    ""table_json"": [
        //    { ""Feature used"": ""Rotating Proxy without JavaScript rendering"", ""API credit cost"": ""1""},
        //    { ""Feature used"": ""Rotating Proxy with JavaScript rendering(default)"", ""API credit cost"": ""5""},
        //    { ""Feature used"": ""Premium Proxy without JavaScript rendering"", ""API credit cost"": ""10""},
        //    { ""Feature used"": ""Premium Proxy with JavaScript rendering"", ""API credit cost"": ""25""}
        //    ]
        //}
        var expectedString = @"
""{""table_json"":[{""Feature used"":""Rotating Proxy without JavaScript rendering"",""API credit cost"":""1""},{""Feature used"":""Rotating Proxy with JavaScript rendering (default)"",""API credit cost"":""5""},{""Feature used"":""Premium Proxy without JavaScript rendering"",""API credit cost"":""10""},{""Feature used"":""Premium Proxy with JavaScript rendering"",""API credit cost"":""25""}]}""
";
        var rawHtml = FileHelpers.GetHtml(Html);
        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "table_json", new ExtractRule
                    {
                        Selector = "#pricing_table",
                        ItemType = ItemType.TableJson
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);
        var resultString = JsonSerializer.Serialize(result);
        resultString.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }

    [Fact]
    public void GetTableArray()
    {
        //{
        //    "table_array": [
        //    ["Rotating Proxy without JavaScript rendering", "1"],
        //    ["Rotating Proxy with JavaScript rendering  (default)", "5"],
        //    ["Premium Proxy without JavaScript rendering", "10"],
        //    ["Premium Proxy with JavaScript rendering", "25"]
        //        ]
        //}
        var expectedString = @"
""{""table_array"":[[""Rotating Proxy without JavaScript rendering"",""1""],[""Rotating Proxy with JavaScript rendering (default)"",""5""],[""Premium Proxy without JavaScript rendering"",""10""],[""Premium Proxy with JavaScript rendering"",""25""]]}""
";
        var rawHtml = FileHelpers.GetHtml(Html);
        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "table_array", new ExtractRule
                    {
                        Selector = "#pricing_table",
                        ItemType = ItemType.TableArray
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);
        var resultString = JsonSerializer.Serialize(result);
        resultString.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }

    [Fact]
    public void AllSelectors()
    {
//{
//	"title_text": "Documentation - Data Extraction",
//	"title_html": "Documentation - Data Extraction",
//	"table_array": [
//		[
//			"Rotating Proxy without JavaScript rendering",
//			"1"
//		],
//		[
//			"Rotating Proxy with JavaScript rendering (default)",
//			"5"
//		],
//		[
//			"Premium Proxy without JavaScript rendering",
//			"10"
//		],
//		[
//			"Premium Proxy with JavaScript rendering",
//			"25"
//		]
//	],
//	"table_json": [
//		{
//			"Feature used": "Rotating Proxy without JavaScript rendering",
//			"API credit cost": "1"
//		},
//		{
//			"Feature used": "Rotating Proxy with JavaScript rendering (default)",
//			"API credit cost": "5"
//		},
//		{
//			"Feature used": "Premium Proxy without JavaScript rendering",
//			"API credit cost": "10"
//		},
//		{
//			"Feature used": "Premium Proxy with JavaScript rendering",
//			"API credit cost": "25"
//		}
//	]
//}
        var expectedString = @"
""{""title_text"":""Documentation - Data Extraction"",""title_html"":""Documentation - Data Extraction"",""table_array"":[[""Rotating Proxy without JavaScript rendering"",""1""],[""Rotating Proxy with JavaScript rendering (default)"",""5""],[""Premium Proxy without JavaScript rendering"",""10""],[""Premium Proxy with JavaScript rendering"",""25""]],""table_json"":[{""Feature used"":""Rotating Proxy without JavaScript rendering"",""API credit cost"":""1""},{""Feature used"":""Rotating Proxy with JavaScript rendering (default)"",""API credit cost"":""5""},{""Feature used"":""Premium Proxy without JavaScript rendering"",""API credit cost"":""10""},{""Feature used"":""Premium Proxy with JavaScript rendering"",""API credit cost"":""25""}]}""
";
        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title_text", new ExtractRule
                    {
                        Selector = "h1",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "title_html", new ExtractRule
                    {
                        Selector = "h1",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "table_array", new ExtractRule
                    {
                        Selector = "table",
                        ItemType = ItemType.TableArray
                    }
                },
                {
                    "table_json", new ExtractRule
                    {
                        Selector = "table",
                        ItemType = ItemType.TableJson
                    }
                }
            }
        };


        var rawHtml = FileHelpers.GetHtml(Html);

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);
        var resultString = JsonSerializer.Serialize(result);
        resultString.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }


    [Fact]
    public void SingleItemOrList()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "first_post_title", new ExtractRule
                    {
                        Selector = ".post-title",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "all_post_title", new ExtractRule
                    {
                        Selector = ".post-title",
                        ItemType = ItemType.List
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        //{
        //    "first_post_title": "  Block ressources with Puppeteer - (5min)",
        //    "all_post_title": [
        //    "  Block ressources with Puppeteer - (5min)",
        //    "  Web Scraping vs Web Crawling: Ultimate Guide - (10min)",
        //    ...
        //    "  Scraping E-Commerce Product Data - (6min)",
        //    "  Introduction to Chrome Headless with Java - (4min)"
        //        ]
        //}
    }

    [Fact]
    public void CleanTextTrueTest()
    {
        //{
        //    "first_post_description" : {
        //        "selector": ".card > div",
        //        "clean": true #default
        //    }
        //}


        //{
        //    "first_post_description": "How to Use a Proxy with Python Requests? - (7min) By Maxine Meurer 13 October 2021 In this tutorial we will see how to use a proxy with the Requests package. We will also discuss on how to choose the right proxy provider.read more",
        //}
    }

    [Fact]
    public void CleanTextFalseTest()
    {
        //{
        //    "first_post_description" : {
        //        "selector": ".card > div",
        //        "clean": false
        //    }
        //}

        //{
        //    "first_post_description": "\n                How to Use a Proxy with Python Requests? - (7min)\n        \n            \n            \n            By Maxine Meurer\n            \n            \n            13 October 2021\n            \n        \n        In this tutorial we will see how to use a proxy with the Requests package. We will also discuss on how to choose the right proxy provider.\n        read more\n        ",
        //}
    }

    [Fact]
    public void NestedObject()
    {
        //{
        //    "title" : "h1",
        //    "subtitle" : "#subtitle",
        //    "articles": {
        //        "selector": ".card",
        //        "type": "list",
        //        "output": {
        //            "title": ".post-title",
        //            "link": {
        //                "selector": ".post-title",
        //                "output": "@href"
        //            },
        //            "description": ".post-description"
        //        }
        //    }
        //}


        //{
        //    "title": "The ScrapingBee Blog",
        //    "subtitle": " We help you get better at web-scraping: detailed tutorial, case studies and \n                        writing by industry experts",
        //    "articles": [
        //    {
        //        "title": "  Block ressources with Puppeteer - (5min)",
        //        "link": "https://www.scrapingbee.com/blog/block-requests-puppeteer/",
        //        "description": "This article will show you how to intercept and block requests with Puppeteer using the request interception API and the puppeteer extra plugin."
        //    },
        //    ...
        //    {
        //        "title": "  Web Scraping vs Web Crawling: Ultimate Guide - (10min)",
        //        "link": "https://www.scrapingbee.com/blog/scraping-vs-crawling/",
        //        "description": "What is the difference between web scraping and web crawling? That's exactly what we will discover in this article, and the different tools you can use."
        //    },
        //    ]
        //}
    }

    [Fact]
    public void ExtractAllLinksFromPage()
    {
        //{
        //    "all_links" : {
        //        "selector": "a",
        //        "type": "list",
        //        "output": "@href"
        //    }
        //}

        //{
        //    "all_links": [
        //    "https://www.scrapingbee.com/",
        //    ...,
        //    "https://www.scrapingbee.com/api-store/"
        //        ]
        //}
    }

    [Fact]
    public void ExtractAllLInksAndAnchors()
    {
        //{
        //    "all_links" : {
        //        "selector": "a",
        //        "type": "list",
        //        "output": {
        //            "anchor": "a",
        //            "href": {
        //                "selector": "a",
        //                "output": "@href"
        //            }
        //        }
        //    }
        //}

        //{
        //    "all_links":[
        //    {
        //        "anchor":"Blog",
        //        "href":"https://www.scrapingbee.com/blog/"
        //    },
        //    ...
        //    {
        //        "anchor":" Linkedin ",
        //        "href":"https://www.linkedin.com/company/26175275/admin/"
        //    }
        //    ]
        //}
    }

    [Fact]
    public void ExtractAllEmails()
    {
        //{
        //    "email_addresses": {
        //        "selector": "a[href^='mailto']",
        //        "output": "@href",
        //        "type": "list"
        //    }
        //}

        //{
        //    "email_addresses": [
        //    "mailto:contact@scrapingbee.com"
        //        ]
        //}
    }
}