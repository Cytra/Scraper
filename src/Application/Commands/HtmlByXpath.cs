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
        private readonly ISeleniumService _seleniumService;
        private readonly IHtmlToJsonService _htmlToJsonService;

        public Handler(
            ISeleniumService seleniumService, 
            IHtmlToJsonService htmlToJsonService)
        {
            _seleniumService = seleniumService;
            _htmlToJsonService = htmlToJsonService;
        }

        public Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var html = _seleniumService.GetData(request.Url);

            var json = _htmlToJsonService.GetDictionaryFromHtml(html);
            return Task.FromResult(new Response()
            {
                Json = json
            });
        }
    }
}