using AnyAct.Extensions;
using AnyAct.Implementations;
using AnyAct.IntegrationTests.Models;
using AnyAct.Interfaces;
using AnyAct.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AnyAct.IntegrationTests.Tests;

public class ServiceCollectionExtensionsTests : IAsyncLifetime
{
    private readonly IServiceCollection _services;

    public ServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddAnyAct_RegistersCorrectHandlers()
    {
        // Act
        _services.AddAnyAct<ServiceCollectionExtensionsTests>();

        // Assert
        var serviceProvider = _services.BuildServiceProvider();

        // Check that each handler is correctly registered.
        foreach (var pair in ActionHandlerCache.Cache)
        {
            var handler = serviceProvider.GetService(pair.Value);
            handler.Should().NotBeNull();
            var actionType = pair.Key.Item1;
            
            if (actionType == typeof(MyAction))
            {
                handler.Should().BeOfType<MyActionHandler>();
            }

            if (actionType == typeof(MyCustomAction))
            {
                handler.Should().BeOfType<CustomActionHandler>();
            }
        }

        // Check that the action executor is correctly registered.
        var actionExecutor = serviceProvider.GetService<IActionExecutor>();
        actionExecutor.Should().NotBeNull();
        actionExecutor.Should().BeOfType<ActionExecutor>();
    }
    
    [Fact]
    public void AddAnyAct_RegistersCorrectCustomHandlers()
    {
        // Arrange
        var customHandlerType = typeof(ICustomActionHandler<>);
        
        // Act
        _services.AddAnyAct<ServiceCollectionExtensionsTests>(customHandlerType);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();

        foreach (var pair in ActionHandlerCache.Cache)
        {
            var handler = serviceProvider.GetService(pair.Value);
            handler.Should().NotBeNull();
            var actionType = pair.Key.Item1;

            actionType.Should().Be(typeof(MyCustomAction));
            handler.Should().BeOfType<CustomActionHandler>();
        }

        // Check that the action executor is correctly registered.
        var actionExecutor = serviceProvider.GetService<IActionExecutor>();
        actionExecutor.Should().NotBeNull();
        actionExecutor.Should().BeOfType<ActionExecutor>();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        ActionHandlerCache.Cache.Clear();
        return Task.CompletedTask;
    }
}