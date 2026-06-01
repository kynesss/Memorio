namespace Memorio.Shared.Exceptions;

public class ValidationException : Exception
{
    private static readonly IReadOnlyDictionary<string, string[]> NoErrors =
        new Dictionary<string, string[]>();

    public ValidationException()
        : this("One or more validation errors occurred.")
    {
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = NoErrors;
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = NoErrors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
