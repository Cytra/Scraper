using Application.Models;
using MediatR;

namespace Application.Queries;

public static class GetHtml
{
    public record Query(string Url) : IRequest<Response>
    { }

    public class Response : ErrorResponse
    {
        public string Html { get; set; }
    }

    public class Handler : IRequestHandler<Query, Response>
    {

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            return new Response();
        }
    }
}