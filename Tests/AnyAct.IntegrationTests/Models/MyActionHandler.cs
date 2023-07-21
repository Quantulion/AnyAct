using AnyAct.Interfaces;

namespace AnyAct.IntegrationTests.Models;

internal sealed class MyActionHandler : IActionHandler<MyAction, MyResult>
{
    public async Task<MyResult> Handle(MyAction value, CancellationToken ct = default)
    {
        return new MyResult();
    }
}