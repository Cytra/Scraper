using Application.Models;
using Application.Services;
using MediatR;

namespace Application.Queries;

public static class GetHtmlByXpath
{
    public class Query : HtmlToJsonByXpath, IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string, object> Json { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly IHtmlToJsonByXpathService _htmlToJsonByXpathService;
        private readonly ISeleniumService _seleniumService;

        public Handler(IHtmlToJsonByXpathService htmlToJsonByXpathService, 
            ISeleniumService seleniumService)
        {
            _htmlToJsonByXpathService = htmlToJsonByXpathService;
            _seleniumService = seleniumService;
        }

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var html = _seleniumService.GetHtml(request.Url);
            var json = _htmlToJsonByXpathService.GetJsonByXpath(request, html);
            return Task.FromResult(new Response()
            {
                Json = json
            });
        }
    }
}