using Application.Models;
using Application.Ports;
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
        private readonly IXpathService _xpathService;
        private readonly ISeleniumService _seleniumService;

        public Handler(IXpathService xpathService, 
            ISeleniumService seleniumService)
        {
            _xpathService = xpathService;
            _seleniumService = seleniumService;
        }

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var html = _seleniumService.GetData(request.Url);
            var json = _xpathService.GetJsonByXpath(request, html);
            return Task.FromResult(new Response()
            {
                Json = json,
            });
        }
    }
}