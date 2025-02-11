namespace RBACAPI.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors, object data)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        Response = data;
    }

    public bool Succeeded { get; init; }
    public string[] Errors { get; init; }
    public object Response { get; set; }

    public static Result Success(object data)
    {
        return new Result(true, Array.Empty<string>(), data);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors, new { });
    }
}
