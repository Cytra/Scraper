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
        var serviceUri = new Uri($"{_seleniumUrl}");
        _logger.LogInformation("Selenium Url {url}", _seleniumUrl);
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
        chromeOptions.AddArgument("--disable-extensions");
        chromeOptions.AddArgument("--disable-popup-blocking");
        chromeOptions.AddArgument("--disable-logging");
        chromeOptions.AddArgument("--disable-notifications");
        chromeOptions.AddArgument("--disable-infobars");
        chromeOptions.AddArgument("--disable-logging");
        chromeOptions.AddArgument("--disable-remote-fonts");
        chromeOptions.AddArgument("--disable-site-isolation-trials");
        chromeOptions.AddArgument("--disable-reading-from-canvas");

        // Set a user agent to mimic a common browser
        chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

        return new RemoteWebDriver(serviceUri, chromeOptions);
    }
}