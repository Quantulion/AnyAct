namespace AnyAct.IntegrationTests.Models;

internal sealed class CustomActionHandler : ICustomActionHandler<MyCustomAction>
{
    public async Task<MyResult> Handle(MyCustomAction value, CancellationToken ct = default)
    {
        return new MyResult();
    }
}