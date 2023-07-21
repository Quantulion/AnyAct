namespace AnyAct.Interfaces;

internal interface IActionHandlerProvider
{
    object GetActionHandler(Type actionModelType, Type customHandlerType);
}