namespace Airbnb.Domain;

public class Result<T>
{
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public T? Value { get; set; }

    private Result(bool isSuccessful, T? value, string? message)
    {
        IsSuccessful = isSuccessful;
        Message = message;
        Value = value;
    }
    
    // Success
    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(true, value, null);
    }
    
    // Failure
    public static implicit operator Result<T>(string error)
    {
        return new Result<T>(false, default, error);
    }
}