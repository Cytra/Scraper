using Application.Interfaces;
using Application.Models;
using Application.Ports;
using MediatR;

namespace Application.Queries;

public static class GetImplicitJson
{
    public class Query : JsonByXpathImplicit, IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string, object?>? Json { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly IHtmlParser<ImplicitExtractRule> _htmlParser;
        private readonly IHtmlService _htmlService;

        public Handler(IHtmlParser<ImplicitExtractRule> htmlParser, 
            IHtmlService htmlService)
        {
            _htmlParser = htmlParser;
            _htmlService = htmlService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var html = await _htmlService.GetData(request.Url);
            var json = _htmlParser.GetJson(request.ExtractRules, html);
            return new Response()
            {
                Json = json,
            };
        }
    }
}