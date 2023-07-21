using AnyAct.Interfaces;

namespace AnyAct.IntegrationTests.Models;

internal sealed class MyActionHandler : IActionHandler<MyAction, MyResult>
{
    public Task<MyResult> Handle(MyAction value, CancellationToken ct = default)
    {
        return Task.FromResult(new MyResult());
    }
}