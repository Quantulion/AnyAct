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
        var resultTypes = new HashSet<Type>();
        
        typeof(TAssemblyMarker).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IActionHandler)) &&
                        t is { IsAbstract: false, IsInterface: false })
            .ToList()
            .ForEach(t =>
            {
                var actionHandlerInterface = t.GetInterface(typeof(IActionHandler<,>).Name)!;
                var actionModelType = actionHandlerInterface.GenericTypeArguments[0];
                var resultType = actionHandlerInterface.GenericTypeArguments[1];
                
                var handlerInterface = t.GetInterface(customHandlerType.Name)!;
                
                services.AddTransient(handlerInterface, t);
                
                resultTypes.Add(resultType);
                ActionHandlerCache.Cache.Add((actionModelType, customHandlerType), handlerInterface);
            });

        var handlerExecutorInterfaceType = typeof(IActionExecutor<>);
        var handlerExecutorType = typeof(ActionExecutor<>);

        foreach (var resultType in resultTypes)
        {
            var interfaceToRegister = handlerExecutorInterfaceType.MakeGenericType(resultType);
            var typeToRegister = handlerExecutorType.MakeGenericType(resultType);

            services.AddSingleton(interfaceToRegister, typeToRegister);
        }

        services.AddSingleton<IActionHandlerProvider, ActionHandlerProvider>();
    }
}