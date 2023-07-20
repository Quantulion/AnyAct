namespace AnyAct.Interfaces;

public interface IActionHandler
{
    
}

public interface IActionHandler<TValue, TResult> : IActionHandler
{
    Task<TResult> Handle(TValue value, CancellationToken ct = default);
}