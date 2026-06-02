using ErrorOr;
using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Memorio.Shared.Results;

public static class ErrorOrExtensions
{
    public static IResult ToResponse<TValue>(this ErrorOr<TValue> result, Func<TValue, IResult>? onValue = null) =>
        result.Match(
            value => onValue is null ? HttpResults.Ok(value) : onValue(value),
            ToProblem);

    private static IResult ToProblem(List<Error> errors)
    {
        if (errors.Count != 0 && errors.TrueForAll(error => error.Type == ErrorType.Validation))
        {
            var failures = errors
                .GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            return HttpResults.ValidationProblem(failures);
        }

        var primary = errors.FirstOrDefault();
        return HttpResults.Problem(statusCode: ToStatusCode(primary.Type), title: primary.Description);
    }

    private static int ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
