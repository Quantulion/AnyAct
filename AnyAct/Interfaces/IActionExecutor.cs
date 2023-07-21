namespace AnyAct.Interfaces;

public interface IActionExecutor
{
    Task<TResult> Execute<TResult>(object value, CancellationToken ct = default);
    Task<TResult> Execute<TResult>(object value, Type customHandlerType, CancellationToken ct = default);
}