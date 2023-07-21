namespace AnyAct.UnitTests.Models;

internal sealed class CustomActionHandler : ICustomActionHandler<MyAction>
{
    public async Task<MyResult> Handle(MyAction value, CancellationToken ct = default)
    {
        return new MyResult();
    }
}