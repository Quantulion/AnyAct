using AnyAct.Exceptions;
using AnyAct.Implementations;
using AnyAct.Interfaces;
using AnyAct.UnitTests.Models;
using AnyAct.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AnyAct.UnitTests.Tests;

public class ActionExecutorTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly ActionExecutor _actionExecutor;

    public ActionExecutorTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        Mock<IServiceScopeFactory> mockServiceScopeFactory = new();
        Mock<IServiceScope> mockServiceScope = new();
        mockServiceScope.Setup(scope => scope.ServiceProvider).Returns(_mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockServiceScope.Object);
        _actionExecutor = new ActionExecutor(mockServiceScopeFactory.Object);
    }

    [Fact]
    public async Task Execute_ReturnsCorrectResult_WhenHandlerExists()
    {
        // Arrange
        var actionType = typeof(MyAction);
        var handlerType = typeof(IActionHandler<MyAction, MyResult>);
        var method = handlerType.GetMethod("Handle");
        ActionHandlerCache.Cache[(actionType, typeof(IActionHandler<,>))] = (handlerType, method!);
        var handler = new MyActionHandler();
        _mockServiceProvider.Setup(sp => sp.GetService(handlerType)).Returns(handler);

        // Act
        var result = await _actionExecutor.Execute<MyResult>(new MyAction());

        // Assert
        result.Should().BeOfType<MyResult>();
    }
    
    [Fact]
    public async Task Execute_ReturnsCorrectResult_WhenCustomHandlerExists()
    {
        // Arrange
        var actionType = typeof(MyAction);
        var handlerType = typeof(ICustomActionHandler<MyAction>);
        var customHandlerType = typeof(ICustomActionHandler<>);
        var method = typeof(IActionHandler<MyAction, MyResult>).GetMethod("Handle");
        ActionHandlerCache.Cache[(actionType, customHandlerType)] = (handlerType, method!);
        var handler = new CustomActionHandler();
        _mockServiceProvider.Setup(sp => sp.GetService(handlerType)).Returns(handler);

        // Act
        var result = await _actionExecutor.Execute<MyResult>(new MyAction(), customHandlerType);

        // Assert
        result.Should().BeOfType<MyResult>();
    }

    [Fact]
    public async Task Execute_ThrowsException_WhenHandlerDoesNotExist()
    {
        // Arrange
        var actionType = typeof(MyAction);
        ActionHandlerCache.Cache.Remove((actionType, typeof(IActionHandler<,>)));

        // Act & Assert
        await _actionExecutor
            .Invoking(async e => 
                await e.Execute<MyResult>(new MyAction(), typeof(IActionHandler<MyAction, MyResult>)))
            .Should().ThrowAsync<IncompatibleActionException>();
    }
}

