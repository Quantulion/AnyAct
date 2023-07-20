using System.Reflection;

namespace AnyAct.Exceptions;

public class IncompatibleActionException : Exception
{
    public IncompatibleActionException(MemberInfo actionDataType) : base(
        $"No handlers found for the action data of type {actionDataType.Name}")
    {
    }
}