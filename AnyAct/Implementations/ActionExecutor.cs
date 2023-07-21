using System.Collections.Concurrent;
using System.Reflection;
using AnyAct.Interfaces;

namespace AnyAct.Implementations;

internal class ActionExecutor<TResult> : IActionExecutor<TResult>
{
    private readonly IActionHandlerProvider _actionHandlerProvider;
    private readonly ConcurrentDictionary<Type, MethodInfo> _handleMethodCache = new();

    public ActionExecutor(IActionHandlerProvider actionHandlerProvider)
    {
        _actionHandlerProvider = actionHandlerProvider;
    }

    public async Task<TResult> Execute(object value, CancellationToken ct = default)
    {
        return await Execute(value, typeof(IActionHandler<,>), ct);
    }

    public async Task<TResult> Execute(object value, Type customHandlerType, CancellationToken ct = default)
    {
        var actionType = value.GetType();

        var handler = _actionHandlerProvider.GetActionHandler(actionType, customHandlerType);
        var handlerType = handler.GetType();

        var method = _handleMethodCache.GetOrAdd(handlerType, _ => handlerType.GetMethod("Handle")!);

        var task = (Task<TResult>)method.Invoke(handler, new[]{value, ct})!;
        return await task;
    }
}