using Application.Ports;

namespace Infrastructure.Scrapers;

public class SeleniumService : ISeleniumService
{
    private readonly ISeleniumDriverFactory _seleniumDriverFactory;

    public SeleniumService(ISeleniumDriverFactory seleniumDriverFactory)
    {
        _seleniumDriverFactory = seleniumDriverFactory;
    }

    public string GetData(string url)
    {
        var driver = _seleniumDriverFactory.GetDriver();
        driver.Url = url;
        driver.Navigate();
        var result = driver.PageSource;
        driver.Dispose();
        return result;
    }
}