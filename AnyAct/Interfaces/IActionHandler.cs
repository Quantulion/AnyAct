namespace AnyAct.Interfaces;

public interface IActionHandler
{
    
}

public interface IActionHandler<TResult, TValue> : IActionHandler
{
    Task<TResult> Handle(TValue value, CancellationToken ct = default);
}