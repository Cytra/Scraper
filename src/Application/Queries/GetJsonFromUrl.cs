using Application.Models;
using Application.Ports;
using Application.Services;
using MediatR;

namespace Application.Queries;

public static class GetJsonFromUrl
{
    public record Query(string Url) : IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string,object> Json { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
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

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var html = await _htmlService.GetData(request.Url);

            var json = _htmlToJsonService.GetDictionaryFromHtml(html);
            return new Response() { Json = json };
        }
    }
}