using Scraper.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Scraper.Samples;

public class HtmlGetParametersSample : IMultipleExamplesProvider<HtmlGetParameters>
{
    public IEnumerable<SwaggerExample<HtmlGetParameters>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Products for Sale",
            new HtmlGetParameters
            {
                Url = "https://htmlpreview.github.io/?https://github.com/rochesterj/flying-ninja/blob/main/products-on-sale.html"
            });
        yield return SwaggerExample.Create(
            "AutoPlius Corola",
            new HtmlGetParameters
            {
                Url = "https://m.autoplius.lt/skelbimai/naudoti-automobiliai?offer_type=1&qt=&qt_autocomplete=&category_id=2&make_id%5B44%5D=253"
            });
    }
}