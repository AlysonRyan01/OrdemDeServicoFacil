namespace OrdemDeServicoFacil.Domain.Shared;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }
    public T Value { get; private set; }

    protected Result(T value)
    {
        IsSuccess = true;
        Error = null;
        Value = value;
    }

    protected Result(string error)
    {
        IsSuccess = false;
        Error = error;
        Value = default!;
    }
    
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Fail(string error) => new(error);
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }

    protected Result()
    {
        IsSuccess = true;
        Error = null;
    }

    protected Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }
    
    public static Result Success() => new();
    public static Result Fail(string error) => new(error);
}