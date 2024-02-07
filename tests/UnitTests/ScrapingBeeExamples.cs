using Application.Models;
using Application.Services;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using UnitTests.Helpers;
using Xunit;

namespace UnitTests;

public class ScrapingBeeExamples
{
    private readonly IFixture _fixture;
    private readonly HtmlParser _sut;
    private readonly string _html;
    private const string Html = "ScrapingBeeBlogs";

    public ScrapingBeeExamples()
    {
        _fixture = RealClassFixture.Create();
        _sut = _fixture.Create<HtmlParser>();
        _html = FileHelpers.GetHtml(Html);
    }

    [Fact]
    public async Task ScrapingBeeExamples_OnlyH1TagXpath1()
    {
        var extractRulesString = @"{
    ""title"" : {
        ""selector"": ""//h1"",
        ""OutputType"": ""text"",
        ""type"": ""Item""
    }
}";
        var input = GetInput(extractRulesString);

        var result = _sut.GetJson(input, _html);

        var resultDict = result as Dictionary<string, object>;
        resultDict.First().Key.Should().Be("title");
        resultDict.First().Value.Should().Be("The ScrapingBee Blog");
    }

    [Fact]
    public async Task ScrapingBeeExamples_OnlyH1TagXpath2()
    {
        var extractRulesString = @"{
    ""title"" : {
        ""selector"": ""//h1[@class=\""mb-21\""]"",
        ""OutputType"": ""text"",
        ""type"": ""Item""
    }
}";
        var input = GetInput(extractRulesString);

        var result = _sut.GetJson(input, _html);

        var resultDict = result as Dictionary<string, object>;
        resultDict.First().Key.Should().Be("title");
        resultDict.First().Value.Should().Be("The ScrapingBee Blog");
    }

    private HtmlToJsonByXpath GetInput(string input)
    {
        var extractRules = JsonConvert
            .DeserializeObject<Dictionary<string, ExtractRule>>(input);

        var result = new HtmlToJsonByXpath
        {
            Url = "url",
            ExtractRules = extractRules
        };
        return result;
    }
}