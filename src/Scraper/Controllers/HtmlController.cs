using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scraper.Models;

namespace Scraper.Controllers;

[ApiController]
[Route("api/v1")]
public class HtmlController : ControllerBase
{
    private readonly IMediator _mediator;

    public HtmlController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("html")]
    public async Task<ActionResult<string>> GetHtml(
        [FromQuery] HtmlGetParameters input,
        CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new GetHtml.Query(input.Url),
                cancellationToken);
        return Ok(result.Html);
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

    [HttpGet("implicit-json")]
    public async Task<ActionResult<JObject>> GetImplicitJson(
        [FromQuery] string url,
        [FromQuery] string extractRules,
        CancellationToken cancellationToken)
    {
        var extractRulesObject = JsonConvert.DeserializeObject<Dictionary<string, ImplicitExtractRule>>(extractRules);
        var result = await _mediator
            .Send(new GetImplicitJson.Query()
            {
                Url = url,
                ExtractRules = extractRulesObject
            },
                cancellationToken);
        return Ok(result.Json);
    }

    [HttpPost("explicit-json")]
    public async Task<ActionResult<JObject>> GetExplicitJson(
        [FromQuery] string url,
        [FromBody] Dictionary<string, ExplicitExtractRule> extractRules,
        CancellationToken cancellationToken)
    {
        //var extractRulesObject = JsonConvert.DeserializeObject<Dictionary<string, ExplicitExtractRule>>(extractRules);
        var result = await _mediator
            .Send(new GetExplicitJson.Query()
                {
                    Url = url,
                    ExtractRules = extractRules
            },
                cancellationToken);
        return Ok(result.Json);
    }
}