using Application.Ports;
using MediatR;
using Application.Models;
using Application.Services;

namespace Application.Commands;

public static class HtmlByXpath
{
    public record Command : IRequest<Response>
    {
        required public string Url { get; set; }
    }

    public class Response : ErrorResponse
    {
        public Dictionary<string, object> Json { get; set; }
    }

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IHtmlService _htmlService;
        private readonly IHtmlToJsonService _htmlToJsonService;

        public Handler(
            IHtmlService htmlService, 
            IHtmlToJsonService htmlToJsonService)
        {
            _htmlService = htmlService;
            _htmlToJsonService = htmlToJsonService;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var html = await _htmlService.GetData(request.Url);

            var json = _htmlToJsonService.GetDictionaryFromHtml(html);
            return new Response()
            {
                Json = json
            };
        }
    }
}