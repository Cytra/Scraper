using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scraper.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Scraper.Controllers;

[ApiController]
[Route("api/v1/json")]
public class JsonParserController: ControllerBase
{
    private readonly IMediator _mediator;

    public JsonParserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("raw")]
    public async Task<ActionResult<JObject>> GetRaw(
        [FromQuery] HtmlGetParameters input,
        CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new GetJsonFromUrl.Query(input.Url),
                cancellationToken);
        return Ok(result.Json);
    }

    [HttpGet("html-to-json")]
    public async Task<ActionResult<JObject>> Get(
        [FromQuery] string url,
        [FromQuery] string extractRules,
        CancellationToken cancellationToken)
    {
        var extractRulesObject = JsonConvert.DeserializeObject<Dictionary<string, ExtractRule>>(extractRules);
        var result = await _mediator
            .Send(new GetHtmlByXpath.Query()
                {
                    Url = url,
                    ExtractRules = extractRulesObject
            },
                cancellationToken);
        return Ok(result.Json);
    }
}