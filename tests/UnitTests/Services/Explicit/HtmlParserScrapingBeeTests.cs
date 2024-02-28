using System.Text.Json;
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

namespace UnitTests.Services.Explicit;

public class HtmlParserScrapingBeeTests
{
    private const string Html = "ScrapingBeeTable";
    private readonly HtmlParser<ExplicitExtractRule> _sut;

    public HtmlParserScrapingBeeTests()
    {
        var fixture = RealClassFixture.Create();
        var logger = fixture.Freeze<ILogger<JsonExtractorFacade<ExplicitExtractRule>>>();
        var selectorService = new ExplicitSelectorService();
        var jsonExtractorFacade = new JsonExtractorFacade<ExplicitExtractRule>(selectorService, logger);
        _sut = new HtmlParser<ExplicitExtractRule>(jsonExtractorFacade);
    }

    [Fact]
    public void GetTitleSubtitle()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "title", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Element = "h1"
                    }
                }
            },
            {
                "titleByClass", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Class = "text-36 md:text-42  font-bold text-black-100 mb-19 leading-a48"
                    }
                }
            },
            {
                "subtitle", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Id = "subtitle"
                    }
                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);

        result.Count.Should().Be(3);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("title");
        keys[1].Should().Be("titleByClass");
        keys[2].Should().Be("subtitle");
        values[0].Should().Be("Documentation - Data Extraction");
        values[1].Should().Be("Documentation - Data Extraction");
        values[2].Should().Be("Extract data with CSS or XPATH selectors");
    }

    [Fact]
    public void GetFirstRef()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "anchors", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Element = "a",
                    },
                    
                }
            },
            {
                "links", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Element = "link",
                    },

                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);

        result.Count.Should().Be(2);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("anchors");
        keys[1].Should().Be("links");
        values[0].Should().Be("/");
        values[1].Should().Be("https://www.scrapingbee.com/index.xml");
    }

    [Fact]
    public void GetAllRefs()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "anchors", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Element = "a",
                    },

                }
            },
            {
                "links", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Element = "link",
                    },

                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);

        result.Count.Should().Be(2);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("anchors");
        keys[1].Should().Be("links");
        
        var anchors = values[0] as List<object>;
        anchors!.Count.Should().Be(80);
        anchors[0].Should().Be("/");

        var links = values[1] as List<object>;
        links!.Count.Should().Be(5);
        links[0].Should().Be("https://www.scrapingbee.com/index.xml");
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

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "table_json", new ExplicitExtractRule
                {
                    ItemType = ItemType.TableJson,
                    Selector = new Selector()
                    {
                        Id = "pricing_table",
                    },

                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);
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

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "table_array", new ExplicitExtractRule
                {
                    ItemType = ItemType.TableArray,
                    Selector = new Selector()
                    {
                        Id = "pricing_table",
                    },

                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);
        var resultString = JsonSerializer.Serialize(result);
        resultString.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }

    [Fact]
    public void AllSelectors()
    {
        var expectedString = @"
    ""{""title_text"":""Documentation - Data Extraction"",""title_html"":""Documentation - Data Extraction"",""table_array"":[[""Rotating Proxy without JavaScript rendering"",""1""],[""Rotating Proxy with JavaScript rendering (default)"",""5""],[""Premium Proxy without JavaScript rendering"",""10""],[""Premium Proxy with JavaScript rendering"",""25""]],""table_json"":[{""Feature used"":""Rotating Proxy without JavaScript rendering"",""API credit cost"":""1""},{""Feature used"":""Rotating Proxy with JavaScript rendering (default)"",""API credit cost"":""5""},{""Feature used"":""Premium Proxy without JavaScript rendering"",""API credit cost"":""10""},{""Feature used"":""Premium Proxy with JavaScript rendering"",""API credit cost"":""25""}]}""
    ";
        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {

                "title_text", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Element = "h1",
                    },

                }
            },
            {

                "title_html", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Class = "text-36 md:text-42  font-bold text-black-100 mb-19 leading-a48"
                    },

                }
            },
            {

                "table_array", new ExplicitExtractRule
                {
                    ItemType = ItemType.TableArray,
                    Selector = new Selector()
                    {
                        Id = "pricing_table",
                    },

                }
            },
            {

                "table_json", new ExplicitExtractRule
                {
                    ItemType = ItemType.TableJson,
                    Selector = new Selector()
                    {
                        Id = "pricing_table",
                    },

                }
            },
        };


        var rawHtml = FileHelpers.GetHtml(Html);

        var result = _sut.GetJson(input, rawHtml);
        var resultString = JsonSerializer.Serialize(result);
        resultString.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }

    [Fact]
    public void SingleItemOrList()
    {
        var rawHtml = FileHelpers.GetHtml(Html);

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {

                "first_post_title", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Class = "post-title",
                    },

                }
            },
            {

                "all_post_title", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Class = "post-title",
                    },

                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);

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

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "first_post_description", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Clean = true,
                    Selector = new Selector()
                    {
                        Class = "card",
                    },

                }
            }
        };

        var result = _sut.GetJson(input, rawHtml);

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

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "first_post_description", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Clean = false,
                    Selector = new Selector()
                    {
                        Class = "card",
                    },

                }
            }
        };

        var result = _sut.GetJson(input, rawHtml);

        result.Count.Should().Be(1);
        var values = result.Values.ToList();
        var keys = result.Keys.ToList();
        keys[0].Should().Be("first_post_description");
        values[0].Should().Be("                How to Use a Proxy with Python Requests? - (7min) By Maxine Meurer 13 October 2021 In this tutorial we will see how to use a proxy with the Requests package. We will also discuss on how to choose the right proxy provider.read more");
    }

    [Fact]
    public void NestedObject()
    {
        var expectedString = @"
""[[{""link"":""/blog/crawling-python/""},{""title"":""Web crawling with Python""},{""description"":""This post will show you how to crawl the web using Python. Web crawling is a powerful technique to collect data from the web by finding all the URLs for one or multiple domains""}],[{""link"":""/blog/web-scraping-youtube/""},{""title"":""How to scrape channel data from YouTube""},{""description"":""Learn how to easily scrape channel data from youtube.com.""}],[{""link"":""/blog/ruby-html-parser/""},{""title"":""Ruby HTML and XML Parsers""},{""description"":""Web scraping comes in handy when collecting large amounts of data from the internet. This roundup shares a list of popular Ruby HTML and XML parsers that you can use to simplify web scraping.""}],[{""link"":""/blog/web-scraping-with-scrapy/""},{""title"":""Easy web scraping with Scrapy""},{""description"":""Scrapy is the most popular Python web scraping framework. In this tutorial we will see how to scrape an E-commerce website with Scrapy from scratch.""}],[{""link"":""/blog/xpath-vs-css-selector/""},{""title"":""XPath vs CSS selectors""},{""description"":""Looking at the differences between XPath expressions and CSS selectors. When it is best to use an XPath expression and when a CSS selector. Advantages and drawbacks of each.""}],[{""link"":""/blog/ruby-faraday-proxy/""},{""title"":""How to use a Proxy with Ruby and Faraday""},{""description"":""Learn how to use a proxy with Ruby and Faraday and prevent your IP from being blacklisted.""}],[{""link"":""/blog/web-scraping-realtor/""},{""title"":""How to scrape data from realtor.com""},{""description"":""Learn how to use selenium to scrape data from realtor.com and evade bot detection.""}],[{""link"":""/blog/using-css-selectors-for-web-scraping/""},{""title"":""Using CSS Selectors for Web Scraping""},{""description"":""We are taking a look at how CSS selectors can help us in web scraping, what their syntax is, how we build an ideal selector string, and how they are supported in mainstream languages.""}],[{""link"":""/blog/web-scraping-idealista/""},{""title"":""How to scrape data from idealista""},{""description"":""Learn how to use selenium to scrape data from Idealista and evade bot detection.""}],[{""link"":""/blog/best-mobile-4g-proxy-provider-webscraping/""},{""title"":""The 6 Best mobile and 4G proxy providers for web scraping""},{""description"":""This post will show you the top 6 mobile and 4g proxy provider for web scraping like ScrapingBee and Airproxy.""}],[{""link"":""/blog/parse-html-nokogiri/""},{""title"":""How to Parse HTML in Ruby with Nokogiri?""},{""description"":""Learn to parse HTML with Ruby and Nokogiri with this step-by-step tutorial. We will see the different ways to scrape the web in Ruby through lots of example with the Nokogiri library.""}],[{""link"":""/blog/web-scraping-booking/""},{""title"":""Web Scraping Booking.com""},{""description"":""Learn how to easily scrape search listings from booking.com.""}],[{""link"":""/blog/web-scraping-twitter/""},{""title"":""How to scrape data from Twitter.com""},{""description"":""Learn how to use selenium to scrape data from Twitter.""}],[{""link"":""/blog/scraping-watir-ruby/""},{""title"":""Using Watir to automate web browsers with Ruby""},{""description"":""In this tutorial, you will learn about browser automation and how to do it using Watir and Ruby.""}]]""
    ";

        var rawHtml = FileHelpers.GetHtml("ScrapingBeeBlogs");

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "title", new ExplicitExtractRule
                {
                    ItemType = ItemType.Item,
                    Selector = new Selector()
                    {
                        Element = "h1",
                    },

                }
            },
            {
                "subtitle", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Element = "h4",
                    },

                }
            },
            {
                "articles", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Class = "w-full sm:w-1/2 p-10 md:p-28 flex",
                    },
                    Output = new Dictionary<string, ExplicitExtractRule>
                    {
                        {
                            "link", new ExplicitExtractRule
                            {
                                ItemType = ItemType.Item,
                                Selector = new Selector()
                                {
                                    Element = "a",
                                },
                            }
                        },
                        {
                            "title", new ExplicitExtractRule
                            {
                                ItemType = ItemType.Item,
                                Selector = new Selector()
                                {
                                    Element = "h4",
                                },
                            }
                        },
                        {
                            "description", new ExplicitExtractRule
                            {
                                ItemType = ItemType.Item,
                                Selector = new Selector()
                                {
                                    Element = "p",
                                },
                            }
                        }
                    }
                }
            },
        };

        var test = JsonSerializer.Serialize(input);

        var result = _sut.GetJson(input, rawHtml);

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

        nested.Trim().Should().Be(expectedString.Trim()[1..^1]);
    }

    [Fact]
    public void ExtractAllLinksFromPage()
    {
        var rawHtml = FileHelpers.GetHtml("ScrapingBeeBlogs");

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "all_links", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Element = "link",
                    },

                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);

        result.Count.Should().Be(1);
        result.FirstOrDefault().Key.Should().Be("all_links");
        var links = result.FirstOrDefault().Value as List<object>;
        links!.Count.Should().Be(6);
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

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "all_links", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Element = "a",
                    },
                    Output = new Dictionary<string, ExplicitExtractRule>
                    {
                        {
                            "anchor", new ExplicitExtractRule
                            {
                                ItemType = ItemType.Item,
                                Selector = new Selector()
                                {
                                    Element = "a",
                                },
                            }
                        },
                        {
                            "href", new ExplicitExtractRule
                            {
                                ItemType = ItemType.Item,
                                Selector = new Selector()
                                {
                                    Element = "href",
                                },
                            }
                        }
                    }
                }
            },
        };


        var result = _sut.GetJson(input, rawHtml);

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

        var input = new Dictionary<string, ExplicitExtractRule>
        {
            {
                "email_addresses", new ExplicitExtractRule
                {
                    ItemType = ItemType.List,
                    Selector = new Selector()
                    {
                        Element = "a",
                    },
                    //Selector = "a[href^='mailto']",
                }
            },
        };

        var result = _sut.GetJson(input, rawHtml);
    }
}