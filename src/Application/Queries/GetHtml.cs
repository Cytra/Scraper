using Application.Models;
using Application.Ports;
using MediatR;

namespace Application.Queries;

public static class GetHtml
{
    public record Query(string Url) : IRequest<Response> { }

    public class Response : ErrorResponse
    {
        public string? Html { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response> 
    {
        private readonly IHtmlService _htmlService;

        public Handler(IHtmlService htmlService)
        {
            _htmlService = htmlService;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            return new Response()
            {
                Html = await _htmlService.GetData(request.Url)
            };
        }
    }
}