using System.Text.Json;
using Application.Models;
using Application.Models.Enums;
using Application.Services;
using Application.Services.Parsers;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Services;

public class GetJsonScrapingBeeTests
{
    private readonly IFixture _fixture; 
    private const string Html = "ScrapingBeeTable";
    private readonly HtmlParser<ImplicitExtractRule> _sut;

    public GetJsonScrapingBeeTests()
    {
        _fixture = RealClassFixture.Create();

        var logger = _fixture.Freeze<ILogger<JsonExtractorFacade<ImplicitExtractRule>>>();
        var selectorService = new ImplicitSelectorService();
        var jsonExtractorFacade = new JsonExtractorFacade<ImplicitExtractRule>(selectorService, logger);
        _sut = new HtmlParser<ImplicitExtractRule>(jsonExtractorFacade);
    }

    [Fact]
    public void GetTitleSubtitle()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "title", new ImplicitExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "h1"
                    }
                },
                {
                    "subtitle", new ImplicitExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "#subtitle"
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

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

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "title", new ImplicitExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "#title"
                    }
                },
                {
                    "title2", new ImplicitExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "//*[@id='title']" //*[@id='yourNodeId']
                    }
                },
                {
                    "title3", new ImplicitExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "//h1[@id='title']"
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

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

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "link", new ImplicitExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "@href"
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(1);
        result.FirstOrDefault().Key.Should().Be("link");
        result.FirstOrDefault().Value.Should().Be("https://www.scrapingbee.com/index.xml");
    }

    [Fact]
    public void GetAllRefs()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "link", new ImplicitExtractRule
                    {
                        ItemType = ItemType.List,
                        Selector = "@href"
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(1);
        result.FirstOrDefault().Key.Should().Be("link");
        var links = result.FirstOrDefault().Value as List<object>;
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
        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "table_json", new ImplicitExtractRule
                    {
                        Selector = "#pricing_table",
                        ItemType = ItemType.TableJson
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);
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
        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "table_array", new ImplicitExtractRule
                    {
                        Selector = "#pricing_table",
                        ItemType = ItemType.TableArray
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);
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
        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "title_text", new ImplicitExtractRule
                    {
                        Selector = "h1",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "title_html", new ImplicitExtractRule
                    {
                        Selector = "h1",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "table_array", new ImplicitExtractRule
                    {
                        Selector = "table",
                        ItemType = ItemType.TableArray
                    }
                },
                {
                    "table_json", new ImplicitExtractRule
                    {
                        Selector = "table",
                        ItemType = ItemType.TableJson
                    }
                }
            }
        };


        var rawHtml = FileHelpers.GetHtml(Html);

        var result = _sut.GetJson(input.ExtractRules, rawHtml);
        var resultString = JsonSerializer.Serialize(result);
        resultString.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }

    [Fact]
    public void SingleItemOrList()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "first_post_title", new ImplicitExtractRule
                    {
                        Selector = ".post-title",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "all_post_title", new ImplicitExtractRule
                    {
                        Selector = ".post-title",
                        ItemType = ItemType.List
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(2);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("first_post_title");
        keys[1].Should().Be("all_post_title");
        values[0].Should().Be("\"  Block ressources with Puppeteer - (5min)\"");
        var postList = values[1] as List<object>;
        postList.Count.Should().Be(4);
        postList[0].Should().Be("\"  Block ressources with Puppeteer - (5min)\"");
        postList[1].Should().Be("\"  Web Scraping vs Web Crawling: Ultimate Guide - (10min)\"");
    }

    [Fact]
    public void CleanTextTrue()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "first_post_description", new ImplicitExtractRule
                    {
                        Selector = ".card",
                        ItemType = ItemType.Item,
                        Clean = true
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(1);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("first_post_description");
        values[0].Should().Be("How to Use a Proxy with Python Requests? - (7min) By Maxine Meurer 13 October 2021 In this tutorial we will see how to use a proxy with the Requests package. We will also discuss on how to choose the right proxy provider.read more");
    }

    [Fact]
    public void DirtyTextFalse()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "first_post_description", new ImplicitExtractRule
                    {
                        Selector = ".card",
                        ItemType = ItemType.Item,
                        Clean = false
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(1);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("first_post_description");
        values[0].Should().Be("                How to Use a Proxy with Python Requests? - (7min) By Maxine Meurer 13 October 2021 In this tutorial we will see how to use a proxy with the Requests package. We will also discuss on how to choose the right proxy provider.read more");
    }

    [Fact]
    public void NestedObject()
    {
        var rawHtml = FileHelpers.GetHtml("ScrapingBeeBlogs");

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "title", new ImplicitExtractRule
                    {
                        Selector = "h1",
                        ItemType = ItemType.Item
                    }
                },
                {
                    "subtitle", new ImplicitExtractRule
                    {
                        Selector = "h4",
                        ItemType = ItemType.List
                    }
                },
                {
                    "articles", new ImplicitExtractRule
                    {
                        Selector = ".w-full sm:w-1/2 p-10 md:p-28 flex",
                        ItemType = ItemType.List,
                        Output = new Dictionary<string, ImplicitExtractRule>
                        {
                            {
                                "link", new ImplicitExtractRule
                                {
                                    Selector = "@href",
                                    ItemType = ItemType.Item,
                                }
                            },
                            {
                                "title", new ImplicitExtractRule
                                {
                                    Selector = "h4",
                                    ItemType = ItemType.Item,
                                }
                            },
                            {
                                "description", new ImplicitExtractRule
                                {
                                    Selector = "p",
                                    ItemType = ItemType.Item,
                                }
                            }
                        }
                    }
                },

            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(3);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("title");
        keys[1].Should().Be("subtitle");
        keys[2].Should().Be("articles");
        values[0].Should().Be("The ScrapingBee Blog");

        var subtitle = values[1] as List<object>;
        subtitle!.Count.Should().Be(23);
        subtitle.FirstOrDefault().Should().Be("Don't know where to begin?");

        var nested = JsonSerializer.Serialize(values[2]);
        nested.Should().Be("[[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}],[{\"link\":\"https://www.scrapingbee.com/index.xml\"},{\"title\":\"Don\\u0027t know where to begin?\"},{\"description\":\"text-30\"}]]");
    }

    [Fact]
    public void ExtractAllLinksFromPage()
    {
        var rawHtml = FileHelpers.GetHtml("ScrapingBeeBlogs");

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "all_links", new ImplicitExtractRule
                    {
                        Selector = "@href",
                        ItemType = ItemType.List
                    }
                },
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        result.Count.Should().Be(1);
        result.FirstOrDefault().Key.Should().Be("all_links");
        var links = result.FirstOrDefault().Value as List<object>;
        links!.Count.Should().Be(109);
        links.FirstOrDefault().Should().Be("https://www.scrapingbee.com/index.xml");
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

        var rawHtml = FileHelpers.GetHtml("ScrapingBeeBlogs");

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "all_links", new ImplicitExtractRule
                    {
                        Selector = "a",
                        ItemType = ItemType.List,
                        Output = new Dictionary<string, ImplicitExtractRule>
                        {
                            {
                                "anchor", new ImplicitExtractRule
                                {
                                    Selector = "a",
                                    ItemType = ItemType.Item,
                                }
                            },
                            {
                                "href", new ImplicitExtractRule
                                {
                                    Selector = "href",
                                    ItemType = ItemType.Item,
                                }
                            }
                        }
                    }
                },
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

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
        //<span class="hljs-string">"mailto:contact@scrapingbee.com"</span>

        var rawHtml = FileHelpers.GetHtml("ScrapingBeeBlogs");

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "email_addresses", new ImplicitExtractRule
                    {
                        Selector = "a[href^='mailto']",
                        ItemType = ItemType.List,
                    }
                },
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);
    }
}