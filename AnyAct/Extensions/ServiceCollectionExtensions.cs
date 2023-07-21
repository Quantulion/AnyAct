using AnyAct.Implementations;
using AnyAct.Interfaces;
using AnyAct.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAnyAct<TAssemblyMarker>(this IServiceCollection services)
    {
        var handlerGenericType = typeof(IActionHandler<,>);
        services.AddAnyAct<TAssemblyMarker>(handlerGenericType);
    }
    
    public static void AddAnyAct<TAssemblyMarker>(this IServiceCollection services, Type customHandlerType)
    {
        typeof(TAssemblyMarker).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IActionHandler)) &&
                        t is { IsAbstract: false, IsInterface: false })
            .ToList()
            .ForEach(t =>
            {
                var actionHandlerInterface = t.GetInterface(typeof(IActionHandler<,>).Name)!;
                var actionModelType = actionHandlerInterface.GenericTypeArguments[0];
                
                var handlerInterface = t.GetInterface(customHandlerType.Name);

                if (handlerInterface is null)
                {
                    return;
                }
                
                services.AddTransient(handlerInterface, t);
                
                ActionHandlerCache.Cache.Add((actionModelType, customHandlerType), handlerInterface);
            });

        services.AddSingleton<IActionExecutor, ActionExecutor>();
    }
}