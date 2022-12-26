using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Queries;
using Newtonsoft.Json.Linq;
using Scraper.Models;

namespace Scraper.Controllers;

[ApiController]
[Route("api/v1/html")]
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
        return Ok(result);
    }

    [HttpGet("jObject")]
    public async Task<ActionResult<JObject>> Get(
        [FromQuery] HtmlGetParameters input,
        CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new GetJObject.Query(input.Url),
                cancellationToken);
        return Ok(result.JObject);
    }
}