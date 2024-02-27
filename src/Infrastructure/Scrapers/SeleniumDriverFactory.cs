using Application.Options;
using Application.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Infrastructure.Scrapers;

public class SeleniumDriverFactory : ISeleniumDriverFactory
{
    private readonly string _seleniumUrl;
    private readonly ILogger<SeleniumDriverFactory> _logger;
    public SeleniumDriverFactory(IOptions<AppOptions> options, ILogger<SeleniumDriverFactory> logger)
    {
        _logger = logger;
        _seleniumUrl = options.Value.SeleniumUrl;
    }
    public RemoteWebDriver GetDriver()
    {
        _logger.LogInformation("Selenium Url {url}", _seleniumUrl);
        var chromeOptions = new ChromeOptions(); 
        return new RemoteWebDriver(new Uri($"{_seleniumUrl}"), chromeOptions);
    }
}