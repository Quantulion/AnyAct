using AnyAct.Interfaces;

namespace AnyAct.UnitTests.Models;

internal sealed class MyActionHandler : IActionHandler<MyAction, MyResult>
{
    public async Task<MyResult> Handle(MyAction value, CancellationToken ct = default)
    {
        return new MyResult();
    }
}