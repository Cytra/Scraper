using Application.Ports;

namespace Infrastructure.Scrapers;

public class HtmlService : IHtmlService
{
    private readonly ISeleniumDriverFactory _seleniumDriverFactory;

    public HtmlService(ISeleniumDriverFactory seleniumDriverFactory)
    {
        _seleniumDriverFactory = seleniumDriverFactory;
    }

    public async Task<string> GetData(string url)
    {
        var driver = _seleniumDriverFactory.GetDriver();
        driver.Url = url;
        driver.Navigate();
        var result = driver.PageSource;
        driver.Dispose();
        return result;
    }
}