using System.Collections.Immutable;

namespace Application.Models;

public record Result<T>
{
    private Result(T value)
    {
        Value = value;
    }

    private Result(params string[] errors)
    {
        Errors = errors.ToImmutableList();
    }

    public bool Succeeded => !HasErrors();
    public bool Failed => HasErrors();
    public T? Value { get; init; }

    public ImmutableList<string> Errors { get; } = ImmutableList<string>.Empty;
    private bool HasErrors()
    {
        return Errors.Count > 0;
    }
    
    public static Result<T> Error(params string[] errors)
    {
        return new Result<T>(errors);
    }
    
    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }
}