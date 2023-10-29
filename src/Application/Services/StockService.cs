using Application.Ports;

namespace Application.Services;

public interface ISeleniumService
{
    string GetStocks();
}

public class StockService : ISeleniumService
{
    private readonly ISeleniumDriverFactory _seleniumDriverFactory;

    public StockService(ISeleniumDriverFactory seleniumDriverFactory)
    {
        _seleniumDriverFactory = seleniumDriverFactory;
    }

    public string GetStocks()
    {
        var driver = _seleniumDriverFactory.GetDriver();
        //driver.Url = url;
        driver.Navigate();
        var result = driver.PageSource;
        driver.Dispose();
        return result;
    }
}