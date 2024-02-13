using Application.Interfaces;
using Application.Models;
using Application.Ports;
using MediatR;
using OneOf;

namespace Application.Queries;

public static class GetExplicitJson
{
    public class Query : JsonByXpathExplicit, IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string, object?>? Json { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly IHtmlParser _htmlParser;
        private readonly ISeleniumService _seleniumService;

        public Handler(IHtmlParser htmlParser,
            ISeleniumService seleniumService)
        {
            _htmlParser = htmlParser;
            _seleniumService = seleniumService;
        }

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var html = _seleniumService.GetData(request.Url);
            var json = _htmlParser.GetJson(request, html);
            return Task.FromResult(new Response()
            {
                Json = json,
            });
        }
    }
}