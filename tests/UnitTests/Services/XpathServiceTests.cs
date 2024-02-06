using System.Reflection;
using Application.Models;
using Application.Models.Enums;
using Application.Services;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Xunit;

namespace UnitTests.Services;

public class XpathServiceTests
{
    private readonly IFixture _fixture;

    public XpathServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void QuotesToScrape_NestedObject()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "products", new ExtractRule
                    {
                        Selector = "//div[@class=\"col-md-8\"]",
                        ItemType = ItemType.Item,
                        OutputType = OutputType.Text,
                        Output = new Dictionary<string, ExtractRule>
                        {
                            {
                                "Quote", new ExtractRule
                                {
                                    Selector = "//span[@class=\"text\"]",
                                    ItemType = ItemType.Item,
                                    OutputType = OutputType.Text
                                }
                            },
                            {
                                "By", new ExtractRule
                                {
                                    Selector = "//small[@class=\"author\"]",
                                    ItemType = ItemType.Item,
                                    OutputType = OutputType.Text
                                }
                            }
                        }
                    }
                }
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        var productDict = result as Dictionary<string, object>;
        var products = productDict["products"];

        var productsSecondLevel = products as List<Dictionary<string, object>>;
        productsSecondLevel.Count.Should().Be(10);

        productsSecondLevel[0]["Quote"].ToString().Should().Be(
            "“The world as we have created it is a process of our thinking. It cannot be changed without changing our thinking.”");
        productsSecondLevel[0]["By"].ToString().Should().Be("Albert Einstein");
    }

    [Fact]
    public void QuotesToScrape_SingleObject()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title", new ExtractRule
                    {
                        Selector = "//div[@class=\"col-md-8\"]",
                        ItemType = ItemType.Item,
                        OutputType = OutputType.Text,
                    }
                }
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        var title = result as Dictionary<string, object>;
        title["title"].Should().Be("Quotes to Scrape");
    }

    [Fact]
    public void QuotesToScrape_SingleObject_MinimalInput()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title", new ExtractRule
                    {
                        Selector = "//div[@class=\"col-md-8\"]",
                    }
                }
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        var title = result as Dictionary<string, object>;
        title["title"].Should().Be("Quotes to Scrape");
    }

    [Fact]
    public void QuotesToScrape_List_10Items()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "products", new ExtractRule
                    {
                        ItemType = ItemType.List,
                        Selector = "//span[@class=\"text\"]",
                    }
                }
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        var title = result as Dictionary<string, object>;
        var products = title["products"];
        var productsList = products as List<object>;
        productsList[0].Should()
            .Be(
                "“The world as we have created it is a process of our thinking. It cannot be changed without changing our thinking.”");
        productsList.Count.Should().Be(10);
    }

    [Fact]
    public void QuotesToScrape_NullExtractRules_EmptyList()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = null
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        var resultDict = result as Dictionary<string, object>;
        resultDict.Count.Should().Be(0);

    }

    [Fact]
    public void ScrapingBee_GetTitle()
    {
        var rawHtml = GetHtml("ScrapingBeeTable");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "h1",
                    }
                }
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        result.FirstOrDefault().Key.Should().Be("title");
        result.FirstOrDefault().Value.Should().Be("Documentation - Data Extraction");
    }

    [Fact]
    public void ScrapingBee_GetAllRefs()
    {
        var rawHtml = GetHtml("ScrapingBeeTable");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "link", new ExtractRule
                    {
                        ItemType = ItemType.List,
                        Selector = "@href",
                    }
                },
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);
    }

    [Fact]
    public void TEst4()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "#title",
                    }
                },
                {
                    "title2", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "//h1[@id=\\\"title\\",
                    }
                },
                {
                    "title3", new ExtractRule
                    {
                        ItemType = ItemType.Item,
                        Selector = "/html/body/h1[@id=\"title\"]",
                    }
                },
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        //{ "extract_rules": { "title": "#title"} } # CSS selector
        //{ "extract_rules": { "title": "//h1[@id=\"title\"]"} } # XPATH selector
        //{ "extract_rules": { "title": "/html/body/h1[@id=\"title\"]"} }  # XPATH selector
    }

    [Fact]
    public async Task Table1Json()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "table_json", new ExtractRule
                    {
                        Selector = "#pricing_table",
                        OutputType = OutputType.TableJson
                    }
                },
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        //output

        //{
        //    "table_json": [
        //    { "Feature used": "Rotating Proxy without JavaScript rendering", "API credit cost": "1"},
        //    { "Feature used": "Rotating Proxy with JavaScript rendering  (default)", "API credit cost": "5"},
        //    { "Feature used": "Premium Proxy without JavaScript rendering", "API credit cost": "10"},
        //    { "Feature used": "Premium Proxy with JavaScript rendering", "API credit cost": "25"}
        //    ]
        //}
    }


    [Fact]
    public async Task Table1Array()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "table_array", new ExtractRule
                    {
                        Selector = "#pricing_table",
                        OutputType = OutputType.TableArray
                    }
                },
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        //output


        //{
        //    "table_array": [
        //    ["Rotating Proxy without JavaScript rendering", "1"],
        //    ["Rotating Proxy with JavaScript rendering  (default)", "5"],
        //    ["Premium Proxy without JavaScript rendering", "10"],
        //    ["Premium Proxy with JavaScript rendering", "25"]
        //        ]
        //}
    }


    [Fact]
    public async Task AllSelectors()
    {
        var rawHtml = GetHtml("QuotesToScrape");

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ExtractRule>
            {
                {
                    "title_text", new ExtractRule
                    {
                        Selector = "h1",
                        OutputType = OutputType.Text
                    }
                },
                {
                    "title_html", new ExtractRule
                    {
                        Selector = "h1",
                        OutputType = OutputType.Html
                    }
                },
                {
                    "table_array", new ExtractRule
                    {
                        Selector = "table",
                        OutputType = OutputType.TableArray
                    }
                },
                {
                    "table_json", new ExtractRule
                    {
                        Selector = "table",
                        OutputType = OutputType.TableJson
                    }
                },
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

        //{
        //    "title_text": "Documentation - HTML API",
        //    "title_text_relevant": "Documentation - HTML API", # No particular effect here. Use it on "body" to see the difference with "text"
        //    "title_html": "<h1 id=\"the-scrapingbee-documentation\"> Documentation - HTML API </h1>",
        //    "title_id": "the-scrapingbee-documentation"
        //    "table_array": [
        //    ["Rotating Proxy without JavaScript rendering", "1"],
        //    ["Rotating Proxy with JavaScript rendering  (default)", "5"],
        //    ["Premium Proxy without JavaScript rendering", "10"],
        //    ["Premium Proxy with JavaScript rendering", "25"]
        //        ]
        //    "table_json": [
        //    { "Feature used": "Rotating Proxy without JavaScript rendering", "API credit cost": "1"},
        //    { "Feature used": "Rotating Proxy with JavaScript rendering  (default)", "API credit cost": "5"},
        //    { "Feature used": "Premium Proxy without JavaScript rendering", "API credit cost": "10"},
        //    { "Feature used": "Premium Proxy with JavaScript rendering", "API credit cost": "25"}
        //    ]
        //}
    }


    public async Task SingleItemOrList()
    {

        var rawHtml = GetHtml("QuotesToScrape");

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
                },
            }
        };

        var sut = _fixture.Create<XpathService>();

        var result = sut.GetJsonByXpath(input, rawHtml);

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

    public async Task CleanTextTrueTest()
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

    public async Task CleanTextFalseTest()
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

    public async Task NestedObject()
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


    public async Task ExtractAllLinksFromPage()
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

    public async Task ExtractAllLInksAndAnchors()
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
    public async Task ExtractAllEmails()
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


    private string GetHtml(string filename)
    {
        var rootFolder = "Data";
        var path = Path.Combine(rootFolder, $"{filename}.html");
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.Combine(assemblyPath, path);
        var rawHtml = File.ReadAllText(fullPath);
        return rawHtml;
    }
}