namespace TeamIndia.TalentFlow.API.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public IEnumerable<string>? Errors { get; }

    public ApiException(string message, int statusCode = 400, IEnumerable<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message = "Resource not found") : base(message, 404) { }
}

public class BadRequestException : ApiException
{
    public BadRequestException(string message = "Bad request", IEnumerable<string>? errors = null) : base(message, 400, errors) { }
}

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message = "Unauthorized") : base(message, 401) { }
}

public class ConflictException : ApiException
{
    public ConflictException(string message = "Conflict") : base(message, 409) { }
}
