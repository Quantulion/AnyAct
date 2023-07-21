using AnyAct.Interfaces;

namespace AnyAct.UnitTests.Models;

internal interface ICustomActionHandler<TValue> : IActionHandler<TValue, MyResult>
{
    
}