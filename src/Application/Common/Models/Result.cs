namespace RBACAPI.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors, object data, string title, string message)
    {
        Succeeded = succeeded;
        Errors = [.. errors];
        Response = data;
        Title = title;
        Message = message;
    }

    public bool Succeeded { get; init; }
    public string[] Errors { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object Response { get; set; }

    public static Result Success(string title, string message, object data)
    {
        return new Result(true, [], data, title, message);
    }

    public static Result Failure(string message, IEnumerable<string> errors)
    {
        return new Result(false, errors, new { }, string.Empty, message);
    }
}
