using Application.Models;
using Application.Models.Enums;
using Application.Services;
using Application.Services.Parsers;
using Application.Services.Selector;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests.Services.Implicit;

public class HtmlParserQuotesToScrapeTests
{
    private readonly IFixture _fixture;
    private const string Html = "QuotesToScrape";
    private readonly HtmlParser<ImplicitExtractRule> _sut;

    public HtmlParserQuotesToScrapeTests()
    {
        _fixture = RealClassFixture.Create();
        var logger = _fixture.Freeze<ILogger<JsonExtractorFacade<ImplicitExtractRule>>>();
        var selectorService = new ImplicitSelectorService();
        var jsonExtractorFacade = new JsonExtractorFacade<ImplicitExtractRule>(selectorService, logger);
        _sut = new HtmlParser<ImplicitExtractRule>(jsonExtractorFacade);
    }

    [Fact]
    public void QuotesToScrape_NestedObject()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "products", new ImplicitExtractRule
                    {
                        Selector = "//div[@class=\"col-md-8\"]",
                        ItemType = ItemType.Item,
                        Output = new Dictionary<string, ImplicitExtractRule>
                        {
                            {
                                "Quote", new ImplicitExtractRule
                                {
                                    Selector = "//span[@class=\"text\"]",
                                    ItemType = ItemType.Item,
                                }
                            },
                            {
                                "By", new ImplicitExtractRule
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

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

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

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "title", new ImplicitExtractRule
                    {
                        Selector = "//div[@class=\"col-md-8\"]",
                        ItemType = ItemType.Item,
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        var title = result as Dictionary<string, object>;
        title["title"].Should().Be("Quotes to Scrape");
    }

    [Fact]
    public void QuotesToScrape_SingleObject_MinimalInput()
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
                        Selector = "//div[@class=\"col-md-8\"]",
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        var title = result as Dictionary<string, object>;
        title["title"].Should().Be("Quotes to Scrape");
    }

    [Fact]
    public void QuotesToScrape_List_10Items()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = new Dictionary<string, ImplicitExtractRule>
            {
                {
                    "products", new ImplicitExtractRule
                    {
                        ItemType = ItemType.List,
                        Selector = "//span[@class=\"text\"]",
                    }
                }
            }
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        var title = result as Dictionary<string, object>;
        var products = title["products"];
        var productsList = products as List<object>;
        productsList![0].Should()
            .Be(
                "“The world as we have created it is a process of our thinking. It cannot be changed without changing our thinking.”");
        productsList.Count.Should().Be(10);
    }

    [Fact]
    public void QuotesToScrape_NullExtractRules_EmptyList()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new JsonByXpathImplicit
        {
            Url = "url",
            ExtractRules = null
        };

        var result = _sut.GetJson(input.ExtractRules, rawHtml);

        var resultDict = result as Dictionary<string, object>;
        resultDict.Count.Should().Be(0);
    }
}