using System.Collections.Concurrent;
using System.Reflection;
using AnyAct.Exceptions;
using AnyAct.Interfaces;
using AnyAct.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.Implementations;

internal class ActionExecutor : IActionExecutor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ConcurrentDictionary<Type, MethodInfo> _handleMethodCache = new();

    public ActionExecutor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResult> Execute<TResult>(object value, CancellationToken ct = default)
    {
        return await Execute<TResult>(value, typeof(IActionHandler<,>), ct);
    }

    public async Task<TResult> Execute<TResult>(object value, Type customHandlerType, CancellationToken ct = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        
        var actionType = value.GetType();
        
        if (!ActionHandlerCache.Cache.TryGetValue((actionType, customHandlerType), out var handlerType))
        {
            throw new IncompatibleActionException(actionType);
        }

        var handler = serviceProvider.GetRequiredService(handlerType);

        var method = _handleMethodCache.GetOrAdd(handlerType, _ => handler.GetType().GetMethod("Handle")!);

        var task = (Task<TResult>)method.Invoke(handler, new[]{value, ct})!;
        return await task;
    }
}