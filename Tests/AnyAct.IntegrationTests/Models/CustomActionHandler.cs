namespace AnyAct.IntegrationTests.Models;

internal sealed class CustomActionHandler : ICustomActionHandler<MyCustomAction>
{
    public Task<MyResult> Handle(MyCustomAction value, CancellationToken ct = default)
    {
        return Task.FromResult(new MyResult());
    }
}