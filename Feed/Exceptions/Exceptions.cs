namespace Feed.Exceptions
{
    public class NotFoundException(string code, string message) : Exception(message)
    {
        public string Code { get; } = code;
    }

    public class Error
    {
        public required string Code { get; init; }

        public string? Description { get; init; }
    }

    public class ValidationException(string code, string message, List<Error> errors) : Exception(message)
    {
        public string Code { get; } = code;

        public List<Error> Errors { get; } = errors;
    }

    public class InternalException(string message, Exception exception = null) : Exception(message, exception);
}
