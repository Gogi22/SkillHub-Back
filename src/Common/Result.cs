namespace Common;

public class Result
{
    protected Result(bool isSuccess, params Error[] error)
    {
        if ((isSuccess && error.Length != 0) || (!isSuccess && error.Length == 0) ||
            (!isSuccess && error[0] == Error.None))
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Errors = error;
    }

    public bool IsSuccess { get; }
    public IEnumerable<Error> Errors { get; }

    public static Result Success()
    {
        return new Result(true, Error.None);
    }

    public static Result Failure(params Error[] errors)
    {
        return new Result(false, errors);
    }

    public static implicit operator Result(Error[] errors)
    {
        return Failure(errors);
    }
    
    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }

    protected static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(value, true, Array.Empty<Error>());
    }

    protected static Result<TValue> Failure<TValue>(params Error[] errors)
    {
        return new Result<TValue>(default, false, errors);
    }
}

public class Result<TValue> : Result
{
    protected internal Result(TValue? value, bool isSuccess, Error[] errors) : base(isSuccess, errors)
    {
        Value = value;
    }

    public TValue? Value { get; private set; }

    public static implicit operator Result<TValue>(TValue value)
    {
        return Success(value);
    }

    public static implicit operator Result<TValue>(Error error)
    {
        return Failure<TValue>(error);
    }

    public static implicit operator Result<TValue>(Error[] errors)
    {
        return Failure<TValue>(errors);
    }
}