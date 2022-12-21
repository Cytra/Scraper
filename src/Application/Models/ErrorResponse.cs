using Application.Models.Enums;

namespace Application.Models;

public class ErrorResponse
{
    public List<Error> Errors { get; set; } = new();
}

public class Error
{
    public ErrorCodes ErrorCode { get; set; }
    public required string ErrorMessage { get; set; }
}