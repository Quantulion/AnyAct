# AnyAct

AnyAct is a flexible, lightweight library for .NET designed to simplify the process of handling different types of actions in a dynamic way at runtime. Modeled after the Mediator pattern, it allows your application to dispatch requests to the correct handler without the need to know the request type at compile time.

## Key Features

- **Runtime Type Resolution**: No need to know the type at compile time. AnyAct allows you to determine the type of request at runtime, providing flexibility in managing your application's workflow.
- **Flexible Handler Dispatch**: Your request handlers are dynamically dispatched based on the runtime type. Handle a variety of scenarios with ease.
- **Easy Integration**: AnyAct works smoothly with your existing dependency injection setup, ensuring you can integrate it into your projects without hassle.
- **Extensible Design**: Built with extensibility in mind, AnyAct allows you to easily extend its functionality to meet your specific application needs.

## Differences with MediatR

While MediatR is a great library for implementing the Mediator pattern, it requires you to know the type of requests at compile time. AnyAct offers a more flexible approach by allowing you to determine the type of request at runtime. This flexibility makes AnyAct a powerful tool for handling a variety of scenarios where the request type cannot be determined at compile time. For example:

```csharp
public async Task<ExecutorResult> ExecuteAction(MyAction action, CancellationToken ct)
{
    // Some preprocessing...
    
    var genericActionType = typeof(MyAction<>).MakeGenericType(modelType);
    var genericAction = Activator.CreateInstance(genericActionType, action)!;

    return await _actionExecutor.Execute<ExecutorResult>(genericAction, ct);
}
```

## Getting Started

To start using AnyAct, you need to install the package and register it in your application's startup configuration.

### Installation

You can install AnyAct via NuGet package manager. Run the following command:

```shell
dotnet add package AnyAct
```

### Registering AnyAct

In your application's startup configuration, you need to add AnyAct services. You can do this in your `Startup.cs` or wherever you configure your services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAnyAct<Startup>();
}
```

The generic type parameter `<TAssemblyMarker>` should be a type that is part of the assembly where your action handlers are located.

### Implementing Handlers

Each handler should implement the `IActionHandler<TValue, TResult>` interface. Here's an example:

```csharp
public class MyActionHandler : IActionHandler<MyAction, MyResult>
{
    public async Task<MyResult> Handle(MyAction action, CancellationToken ct)
    {
        // Handle the action and return the result...
    }
}
```

### Executing Actions

To execute an action, inject `IActionExecutor` into your class and call the `Execute<TResult>` method:

```csharp
public class MyClass
{
    private readonly IActionExecutor _actionExecutor;

    public MyClass(IActionExecutor actionExecutor)
    {
        _actionExecutor = actionExecutor;
    }

    public async Task DoSomething()
    {
        var action = new MyAction();
        var result = await _actionExecutor.Execute<MyResult>(action);
        
        // Use the result...
    }
}
```