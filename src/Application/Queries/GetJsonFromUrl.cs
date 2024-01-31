using Application.Models;
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
        private readonly ISeleniumService _seleniumService;
        private readonly IHtmlToJsonService _htmlToJsonService;

        public Handler(
            ISeleniumService seleniumService,
            IHtmlToJsonService htmlToJsonService)
        {
            _seleniumService = seleniumService;
            _htmlToJsonService = htmlToJsonService;
        }

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
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