using OpenQA.Selenium.Remote;

namespace Application.Ports;

public interface ISeleniumDriverFactory
{
    RemoteWebDriver GetDriver();
}