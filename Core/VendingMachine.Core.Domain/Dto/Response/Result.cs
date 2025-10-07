namespace VendingMachine.Core.Domain.Dto.Response;


public class Result
{
    public bool IsSuccess  { get; }    
    public string[] Messages  { get; } = [];

    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
    
    public Result(bool isSuccess, string[]? messages = null)
    {
        IsSuccess = isSuccess;
        if (messages is not null)
        {
            Messages = messages;
        }
    }
    
    public static Result Success() => new Result(true);

    public static Result Failure(string[]? messages = null) => new Result(false, messages);
}


public class Result<T> : Result
{ 
    public T Value { get; }
    
    public Result(T value, bool isSuccess) : base(isSuccess)
    {
        Value = value;
    } 

    public Result(T value, bool isSuccess, string[]? messages = null) : base(isSuccess, messages)
    {
        Value = value;
    }
    
    public static Result<T> Success(T value) => new Result<T>(value, true);
    
    public new static Result<T> Failure(string[]? messages = null) => new Result<T>(default!, false, messages);
}