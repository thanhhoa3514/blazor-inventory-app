namespace MyApp.Server.Application.Common;

public abstract record AppResult<T>
{
    public sealed record Ok(T Value) : AppResult<T>;
    public sealed record NotFound(string Message) : AppResult<T>;
    public sealed record Conflict(string Message) : AppResult<T>;
    public sealed record ValidationError(string Message) : AppResult<T>;
}
