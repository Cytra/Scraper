using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Queries;
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

    [HttpGet]
    public async Task<ActionResult<string>> Search(
        [FromBody] HtmlGetParameters input,
        CancellationToken cancellationToken)
    {
        var result = await _mediator
            .Send(new GetHtml.Query(input.Url),
                cancellationToken);
        return Ok(result);
    }
}