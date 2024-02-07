using System.Reflection;
using Application.Models;
using Application.Services;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace UnitTests;

public class ScrapingBeeExamples
{
    private readonly IFixture _fixture;
    private readonly HtmlParser _sut;
    private readonly string _html;

    public ScrapingBeeExamples()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _sut = _fixture.Create<HtmlParser>();
        _html = GetHtml();
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

    private string GetHtml()
    {
        var rootFolder = "Data";
        var fileName = "ScrapingBeeBlogs";
        var path = Path.Combine(rootFolder, $"{fileName}.html");
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.Combine(assemblyPath, path);
        var rawHtml = File.ReadAllText(fullPath);
        return rawHtml;
    }
}