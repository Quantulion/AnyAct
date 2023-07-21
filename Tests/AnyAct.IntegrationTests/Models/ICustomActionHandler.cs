using AnyAct.Interfaces;

namespace AnyAct.IntegrationTests.Models;

internal interface ICustomActionHandler<TValue> : IActionHandler<TValue, MyResult>
{
    
}