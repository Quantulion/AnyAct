namespace AnyAct.Interfaces;

public interface IActionExecutor<TResult>
{
    Task<TResult> Execute(IActionData value, CancellationToken ct = default);
    Task<TResult> Execute(IActionData value, Type customHandlerType, CancellationToken ct = default);
}