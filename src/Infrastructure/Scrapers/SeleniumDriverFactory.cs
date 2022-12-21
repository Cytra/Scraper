using Application.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Infrastructure.Scrapers;

public interface ISeleniumDriverFactory
{
    RemoteWebDriver GetDriver();
}

public class SeleniumDriverFactory : ISeleniumDriverFactory
{
    private readonly string _seleniumUrl;
    public SeleniumDriverFactory(IOptions<AppOptions> options)
    {
        _seleniumUrl = options.Value.SeleniumUrl;
    }
    public RemoteWebDriver GetDriver()
    {
        var chromeOptions = new ChromeOptions();
        return new RemoteWebDriver(new Uri(_seleniumUrl), chromeOptions);
    }
}