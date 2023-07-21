namespace AnyAct.UnitTests.Models;

internal sealed class CustomActionHandler : ICustomActionHandler<MyAction>
{
    public Task<MyResult> Handle(MyAction value, CancellationToken ct = default)
    {
        return Task.FromResult(new MyResult());
    }
}