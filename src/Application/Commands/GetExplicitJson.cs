using Application.Interfaces;
using Application.Models;
using Application.Ports;
using MediatR;

namespace Application.Commands;

public static class GetExplicitJson
{
    public class Command : JsonByXpathExplicit, IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string, object?>? Json { get; set; }
    }

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IHtmlParser<ExplicitExtractRule> _htmlParser;
        private readonly IHtmlService _htmlService;

        public Handler(IHtmlParser<ExplicitExtractRule> htmlParser,
            IHtmlService htmlService)
        {
            _htmlParser = htmlParser;
            _htmlService = htmlService;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var html = await _htmlService.GetData(request.Url, request.WaitTime);
            var json = _htmlParser.GetJson(request.ExtractRules, html);
            return new Response()
            {
                Json = json,
            };
        }
    }
}