using AnyAct.Exceptions;
using AnyAct.Interfaces;
using AnyAct.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.Implementations;

internal class ActionHandlerProvider : IActionHandlerProvider
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ActionHandlerProvider(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public object GetActionHandler(Type actionModelType, Type customHandlerType)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        if (!ActionHandlerCache.Cache.TryGetValue((actionModelType, customHandlerType), out var handlerType))
        {
            throw new IncompatibleActionException(actionModelType);
        }

        var handler = serviceProvider.GetRequiredService(handlerType);

        return handler;
    }
}