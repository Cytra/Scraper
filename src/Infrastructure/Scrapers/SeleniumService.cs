using Application.Ports;
using OpenQA.Selenium.Remote;

namespace Infrastructure.Scrapers;

public class HtmlService : IHtmlService
{
    private readonly ISeleniumDriverFactory _seleniumDriverFactory;

    public HtmlService(ISeleniumDriverFactory seleniumDriverFactory)
    {
        _seleniumDriverFactory = seleniumDriverFactory;
    }

    public async Task<string> GetData(string url, int? waitTime = null)
    {
        string result;
        RemoteWebDriver? driver = null;
        try
        {
            driver = _seleniumDriverFactory.GetDriver();
            if (waitTime.HasValue)
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(waitTime.Value);
            }
            driver.Url = url;
            driver.Navigate();

            result = driver.PageSource;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            driver?.Dispose();
        }
        return result;
    }
}