using AnyAct.Implementations;
using AnyAct.Interfaces;
using AnyAct.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.Extensions;

public static class ServicesConfiguration
{
    public static void AddAnyAct<TAssemblyMarker>(this IServiceCollection services)
    {
        var handlerGenericType = typeof(IActionHandler<,>);
        services.AddAnyAct<TAssemblyMarker>(handlerGenericType);
    }

    public static void AddAnyAct<TAssemblyMarker>(this IServiceCollection services, Type customHandlerType)
    {
        var genericHandlerType = typeof(IActionHandler<,>);

        typeof(TAssemblyMarker).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IActionHandler)) &&
                        t is { IsAbstract: false, IsInterface: false })
            .ToList()
            .ForEach(t =>
            {
                var implementedActionHandlerInterfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == customHandlerType);

                foreach (var interfaceType in implementedActionHandlerInterfaces)
                {
                    services.AddTransient(interfaceType, t);

                    var actionHandlerInterfaces = interfaceType.GetGenericTypeDefinition() == genericHandlerType
                        ? new List<Type>() {interfaceType}
                        : interfaceType
                            .GetInterfaces()
                            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericHandlerType)
                            .ToList();

                    foreach (var actionHandlerInterface in actionHandlerInterfaces)
                    {
                        var actionModelType = actionHandlerInterface.GenericTypeArguments[0];

                        var method = actionHandlerInterface.GetMethod("Handle")!;

                        ActionHandlerCache.Cache.Add((actionModelType, customHandlerType), (interfaceType, method));
                    }
                }
            });

        services.AddSingleton<IActionExecutor, ActionExecutor>();
    }
}