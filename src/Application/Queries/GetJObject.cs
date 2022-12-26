using Application.Models;
using Application.Services;
using MediatR;
using Newtonsoft.Json.Linq;

namespace Application.Queries;

public static class GetJObject
{
    public record Query(string Url) : IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public Dictionary<string,object> JObject { get; set; }
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
            var jObject = _jObjectService.GetDictionaryFromHtml(html);
            return new Response()
            {
                JObject = jObject
            };
        }
    }
}