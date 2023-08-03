using AnyAct.Exceptions;
using AnyAct.Interfaces;
using AnyAct.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.Implementations;

internal class ActionExecutor : IActionExecutor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ActionExecutor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<TResult> Execute<TResult>(object value, CancellationToken ct = default)
    {
        return await Execute<TResult>(value, typeof(IActionHandler<,>), ct).ConfigureAwait(false);
    }

    public async Task<TResult> Execute<TResult>(object value, Type customHandlerType, CancellationToken ct = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var actionType = value.GetType();

        if (!ActionHandlerCache.Cache.TryGetValue((actionType, customHandlerType), out var cachedInfo))
        {
            throw new IncompatibleActionException(actionType);
        }

        var handler = serviceProvider.GetRequiredService(cachedInfo.ServiceType);

        var task = (Task<TResult>)cachedInfo.HandleMethodInfo.Invoke(handler, new[] { value, ct })!;
        return await task.ConfigureAwait(false);
    }
}