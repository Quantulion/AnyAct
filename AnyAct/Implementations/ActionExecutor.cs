using AnyAct.Exceptions;
using AnyAct.Interfaces;

namespace AnyAct.Implementations;

public class ActionExecutor<TResult> : IActionExecutor<TResult>
{
    private readonly IServiceProvider _serviceProvider;

    public ActionExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> Execute(IActionData value, CancellationToken ct = default)
    {
        return await Execute(value, typeof(IActionHandler<,>), ct);
    }

    public async Task<TResult> Execute(IActionData value, Type customHandlerType, CancellationToken ct = default)
    {
        var resultType = typeof(TResult);
        var actionType = value.GetType();
        
        var handlerType = customHandlerType.MakeGenericType(resultType, actionType);

        var handler = _serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new IncompatibleActionException(value.GetType());
        }

        var method = handler.GetType().GetMethod("Handle");
        var task = (Task<TResult>)method!.Invoke(handler, new object[]{value, ct})!;
        return await task;
    }
}