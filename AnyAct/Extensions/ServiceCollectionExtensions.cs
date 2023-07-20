using AnyAct.Implementations;
using AnyAct.Interfaces;
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
                var modelType = t.GetInterface(customHandlerType.Name)!.GenericTypeArguments[0];
                var resultType = t.GetInterface(customHandlerType.Name)!.GenericTypeArguments[1];
                services.AddTransient(
                    customHandlerType.MakeGenericType(modelType, resultType), t);
                
                resultTypes.Add(resultType);
            });

        var handlerExecutorInterfaceType = typeof(IActionExecutor<>);
        var handlerExecutorType = typeof(ActionExecutor<>);

        foreach (var resultType in resultTypes)
        {
            var interfaceToRegister = handlerExecutorInterfaceType.MakeGenericType(resultType);
            var typeToRegister = handlerExecutorType.MakeGenericType(resultType);

            services.AddSingleton(interfaceToRegister, typeToRegister);
        }
    }
}