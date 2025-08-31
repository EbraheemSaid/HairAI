namespace HairAI.Application.Common;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public bool Succeeded { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public static Result Success()
    {
        return new Result(true, new string[] { });
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}