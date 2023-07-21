namespace AnyAct.Utils;

internal static class ActionHandlerCache
{
    public static readonly Dictionary<(Type, Type), Type> Cache = new();
}