using System.Reflection;

namespace AnyAct.Utils;

internal static class ActionHandlerCache
{
    public static readonly Dictionary<(Type, Type), (Type ServiceType, MethodInfo HandleMethodInfo)> Cache = new();
}