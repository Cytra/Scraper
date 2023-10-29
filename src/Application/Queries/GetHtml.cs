using Application.Models;
using Application.Services;
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
        private readonly ISeleniumService _seleniumService;

        public Handler(ISeleniumService seleniumService)
        {
            _seleniumService = seleniumService;
        }

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response()
            {
                Html = _seleniumService.GetStocks()
            });
        }
    }
}