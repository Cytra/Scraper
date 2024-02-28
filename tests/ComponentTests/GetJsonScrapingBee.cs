using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Ports;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using ComponentTests.Helpers;
using ComponentTests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System.Text;
using Application.Models;
using Scraper.Models;
using Amazon.Runtime.Internal.Transform;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ComponentTests;

public class GetJsonScrapingBee
{
    private readonly IHtmlService _htmlService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public GetJsonScrapingBee()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _htmlService = fixture.Freeze<IHtmlService>();
        var html = FileHelpers.GetHtml("ScrapingBeeBlogs");
        _htmlService.GetData(Arg.Any<string>()).Returns(html);
    }

    [Fact]
    public async Task ScrapingBeeExamples_OnlyH1TagXpath1()
    {
        await using var factory = new CustomWebApplicationFactory(ConfigureTestServices);
        var client = factory.CreateClient();
        var extractRulesString = @"{
    ""title"" : {
        ""selector"": ""//h1"",
    }
}";

        var response = await client.GetAsync($"/api/v1/implicit-json?Url=http://quotes.toscrape.com&extractRules={extractRulesString}");
        
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonSerializerOptions);
        result!.First().Key.Should().Be("title");
        result!.First().Value.Should().Be("The ScrapingBee Blog");
    }

    [Fact]
    public async Task ScrapingBeeExamples_OnlyH1TagXpath2()
    {
        await using var factory = new CustomWebApplicationFactory(ConfigureTestServices);
        var client = factory.CreateClient();
        var extractRulesString = @"{
    ""title"" : {
        ""selector"": ""//h1[@class=\""mb-21\""]"",
    }
}";

        var response = await client.GetAsync($"/api/v1/implicit-json?Url=http://quotes.toscrape.com&extractRules={extractRulesString}");

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonSerializerOptions);
        result!.First().Key.Should().Be("title");
        result!.First().Value.Should().Be("The ScrapingBee Blog");
    }

    [Fact]
    public async Task Post_ScrapingBeeExamples_OnlyH1TagXpath()
    {
        await using var factory = new CustomWebApplicationFactory(ConfigureTestServices);
        var client = factory.CreateClient();

        var request = new JsonExplicitRequest()
        {
            Url = "http://quotes.toscrape.com",
            ExtractRules = new Dictionary<string, ExplicitExtractRule>()
            {
                {
                    "title", new ExplicitExtractRule()
                    {
                        Selector = new Selector()
                        {
                            Element = "h1"
                        }
                    }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/v1/explicit-json?Url=http://quotes.toscrape.com", content);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonSerializerOptions);
        result!.First().Key.Should().Be("title");
        result!.First().Value.Should().Be("The ScrapingBee Blog");
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services.AddScoped(_ => _htmlService);
    }
}