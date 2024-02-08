using Application.Models;
using Application.Models.Enums;
using Application.Services;
using AutoFixture;
using FluentAssertions;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Services;

public class GetJsonQuotesToScrapeTests
{
    private readonly IFixture _fixture;
    private const string Html = "QuotesToScrape";

    public GetJsonQuotesToScrapeTests()
    {
        _fixture = RealClassFixture.Create();
    }

    [Fact]
    public void QuotesToScrape_NestedObject()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

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
                        Output = new Dictionary<string, ExtractRule>
                        {
                            {
                                "Quote", new ExtractRule
                                {
                                    Selector = "//span[@class=\"text\"]",
                                    ItemType = ItemType.Item,
                                }
                            },
                            {
                                "By", new ExtractRule
                                {
                                    Selector = "//small[@class=\"author\"]",
                                    ItemType = ItemType.Item,
                                }
                            }
                        }
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        var productDict = result as Dictionary<string, object>;
        var products = productDict["products"];

        var productsSecondLevel = products as List<Dictionary<string, object>>;
        productsSecondLevel!.Count.Should().Be(2);

        productsSecondLevel[0]["Quote"].ToString().Should().Be(
            "“The world as we have created it is a process of our thinking. It cannot be changed without changing our thinking.”");
        productsSecondLevel[1]["By"].ToString().Should().Be("Albert Einstein");
    }

    [Fact]
    public void QuotesToScrape_SingleObject()
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
                        Selector = "//div[@class=\"col-md-8\"]",
                        ItemType = ItemType.Item,
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        var title = result as Dictionary<string, object>;
        title["title"].Should().Be("Quotes to Scrape");
    }

    [Fact]
    public void QuotesToScrape_SingleObject_MinimalInput()
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
                        Selector = "//div[@class=\"col-md-8\"]",
                    }
                }
            }
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        var title = result as Dictionary<string, object>;
        title["title"].Should().Be("Quotes to Scrape");
    }

    [Fact]
    public void QuotesToScrape_List_10Items()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

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

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        var title = result as Dictionary<string, object>;
        var products = title["products"];
        var productsList = products as List<string>;
        productsList![0].Should()
            .Be(
                "“The world as we have created it is a process of our thinking. It cannot be changed without changing our thinking.”");
        productsList.Count.Should().Be(10);
    }

    [Fact]
    public void QuotesToScrape_NullExtractRules_EmptyList()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = null
        };

        var sut = _fixture.Create<HtmlParser>();

        var result = sut.GetJson(input, rawHtml);

        var resultDict = result as Dictionary<string, object>;
        resultDict.Count.Should().Be(0);
    }
}