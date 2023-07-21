using AnyAct.Interfaces;

namespace AnyAct.UnitTests.Models;

internal sealed class MyActionHandler : IActionHandler<MyAction, MyResult>
{
    public Task<MyResult> Handle(MyAction value, CancellationToken ct = default)
    {
        return Task.FromResult(new MyResult());
    }
}