﻿using ComponentTests.TestHelpers;
using AutoFixture;
using AutoFixture.AutoMoq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentTests;

public class HtmlControllerPostXpathTests
{
    private readonly IFixture _fixture;


    public HtmlControllerPostXpathTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task ProcessScheduledPayments_HasCandidates_OnlyProducedForScheduled()
    {
        await using var factory = new CustomWebApplicationFactory(ConfigureTestServices);
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/json?Url=http://quotes.toscrape.com&extractRules=");

    }


    private void ConfigureTestServices(IServiceCollection services)
    {
        //services.AddScoped(x => _hostEnvMock.Object);
    }
}