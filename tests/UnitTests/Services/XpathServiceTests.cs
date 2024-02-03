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
        var rawHtml = GetHtml();

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
        var rawHtml = GetHtml();

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

    [Fact] public void QuotesToScrape_SingleObject_MinimalInput()
    {
        var rawHtml = GetHtml();

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
        var rawHtml = GetHtml();

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
        var rawHtml = GetHtml();

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
    public async Task Test1()
    {
        //{"key_name" : "css_or_xpath_selector"}
    }

    [Fact]
    public async Task Test2()
    {
        //{
        //    "title" : "h1",
        //    "subtitle" : "#subtitle",
        //}
    }

    [Fact]
    public async Task Test3()
    {
        //{ "link" : "@href"}
    }

    [Fact]
    public async Task TEst4()
    {
        //{ "extract_rules": { "title": "#title"} } # CSS selector
        //{ "extract_rules": { "title": "//h1[@id=\"title\"]"} } # XPATH selector
        //{ "extract_rules": { "title": "/html/body/h1[@id=\"title\"]"} }  # XPATH selector
    }

    private string GetHtml()
    {
        var rootFolder = "Data";
        var fileName = "QuotesToScrape";
        var path = Path.Combine(rootFolder, $"{fileName}.html");
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.Combine(assemblyPath, path);
        var rawHtml = File.ReadAllText(fullPath);
        return rawHtml;
    }
}