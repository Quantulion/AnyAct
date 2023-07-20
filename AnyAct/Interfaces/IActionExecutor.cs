namespace AnyAct.Interfaces;

public interface IActionExecutor<TResult>
{
    Task<TResult> Execute(object value, CancellationToken ct = default);
    Task<TResult> Execute(object value, Type customHandlerType, CancellationToken ct = default);
}