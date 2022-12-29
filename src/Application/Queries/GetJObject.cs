using Application.Models;
using Application.Services;
using HtmlAgilityPack;
using MediatR;
using Newtonsoft.Json;


namespace Application.Queries;

public static class GetJObject
{
    public record Query(string Url) : IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string,object> Json { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly ISeleniumService _seleniumService;
        private readonly IJObjectService _jObjectService;

        public Handler(
            ISeleniumService seleniumService,
            IJObjectService jObjectService)
        {
            _seleniumService = seleniumService;
            _jObjectService = jObjectService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var html = _seleniumService.GetHtml(request.Url);

            var json = _jObjectService.GetDictionaryFromHtml(html);
            return new Response()
            {
                Json = json
            };
        }
    }
}