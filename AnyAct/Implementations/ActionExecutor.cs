using System.Collections.Concurrent;
using System.Reflection;
using AnyAct.Exceptions;
using AnyAct.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.Implementations;

public class ActionExecutor<TResult> : IActionExecutor<TResult>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<(Type, Type, Type), Type> _handlerTypeCache = new();
    private readonly ConcurrentDictionary<Type, MethodInfo> _handleMethodCache = new();

    public ActionExecutor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResult> Execute(object value, CancellationToken ct = default)
    {
        return await Execute(value, typeof(IActionHandler<,>), ct);
    }

    public async Task<TResult> Execute(object value, Type customHandlerType, CancellationToken ct = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        var actionType = value.GetType();
        var resultType = typeof(TResult);

        var handlerTypeKey = (customHandlerType, actionType, resultType);
        var handlerType = _handlerTypeCache.GetOrAdd(handlerTypeKey, _ => customHandlerType.MakeGenericType(actionType, resultType));

        var handler = serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new IncompatibleActionException(value.GetType());
        }

        var method = _handleMethodCache.GetOrAdd(handlerType, _ => handler.GetType().GetMethod("Handle")!);

        var task = (Task<TResult>)method.Invoke(handler, new[]{value, ct})!;
        return await task;
    }
}