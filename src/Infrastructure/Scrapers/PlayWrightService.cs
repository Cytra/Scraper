using Application.Ports;
using Knyaz.Optimus;
using Knyaz.Optimus.ScriptExecuting.Jint;

namespace Infrastructure.Scrapers;

public class PlayWrightService : IHtmlService
{
    public async Task<string> GetData(string url, int? waitTime = null)
    {
        //using var playwright = await Playwright.CreateAsync();
        //var browser = await playwright.Chromium.LaunchAsync();
        //var context = await browser.NewContextAsync();
        //var page = await context.NewPageAsync();

        //// Example: Navigate to your ASP.NET application
        //await page.GotoAsync(url);

        //var result = await page.InnerHTMLAsync("body");

        //await browser.CloseAsync();
        //return result;

        var engine = EngineBuilder.New()
            .UseJint()// Enable JavaScripts execution.
            .Build(); // Builds the Optimus engine.

        //Request the web page.
        var page = await engine.OpenUrl(url);
        //Get the document
        var document = page.Document.InnerHTML;
        return document;
    }
}