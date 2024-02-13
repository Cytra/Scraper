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

namespace ComponentTests;

public class GetJsonScrapingBee
{
    private readonly ISeleniumService _seleniumService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public GetJsonScrapingBee()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _seleniumService = fixture.Freeze<ISeleniumService>();
        var html = FileHelpers.GetHtml("ScrapingBeeBlogs");
        _seleniumService.GetData(Arg.Any<string>()).Returns(html);
    }

    [Fact]
    public async Task ScrapingBeeExamples_OnlyH1TagXpath1()
    {
        await using var factory = new CustomWebApplicationFactory(ConfigureTestServices);
        var client = factory.CreateClient();
        var extractRulesString = @"{
    ""title"" : {
        ""selector"": ""//h1"",
        ""OutputType"": ""text"",
        ""type"": ""Item""
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
        ""OutputType"": ""text"",
        ""type"": ""Item""
    }
}";

        var response = await client.GetAsync($"/api/v1/implicit-json?Url=http://quotes.toscrape.com&extractRules={extractRulesString}");

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonSerializerOptions);
        result!.First().Key.Should().Be("title");
        result!.First().Value.Should().Be("The ScrapingBee Blog");
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        services.AddScoped(_ => _seleniumService);
    }
}