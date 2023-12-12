public class Result<T>
{
    public bool Success { get; set; }
    public T Value { get; set; }
    public Exception exception { get; set; }
    public Result(bool success, T value)
    {
        Success = success;
        Value = value;
    }
    public Result(bool success)
    {
        Success = success;
    }
    public Result(Exception ex)
    {
        exception = ex;
        Success = false;
    }
    public static Result<T> Failed(Exception ex)
    {
        Result<T> r = new Result<T>(false);
        r.exception = ex;
        return r;
    }
    public static implicit operator Result(Result<T> r) => new(r.Success, r.exception);
    public static implicit operator Result<T>(Result r)
    {
        if (r.Success)
        {
            return new Result<T>(r.Success);
        }
        else
        {
            var r_ = new Result<T>(r.Success)
            {
                exception = r.exception
            };
            return r_;
        }
    }
}

public class Result
{
    public bool Success { get; set; }
    public Exception exception { get; set; }
    public Result(bool success)
    {
        Success = success;
    }
    public Result(Exception ex)
    {
        exception = ex;
        Success = false;
    }
    public Result(bool success, Exception ex)
    {
        Success = success;
        exception = ex;
    }
    public static Result Failed(Exception ex) => new Result(ex);
    public static Result Successful () => new Result(true);
}