using AnyAct.Exceptions;
using AnyAct.Implementations;
using AnyAct.Interfaces;
using AnyAct.UnitTests.Models;
using AnyAct.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AnyAct.UnitTests.Tests;

public class FlexExecutorTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly ActionExecutor _flexExecutor;

    public FlexExecutorTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        Mock<IServiceScopeFactory> mockServiceScopeFactory = new();
        Mock<IServiceScope> mockServiceScope = new();
        mockServiceScope.Setup(scope => scope.ServiceProvider).Returns(_mockServiceProvider.Object);
        mockServiceScopeFactory.Setup(factory => factory.CreateScope()).Returns(mockServiceScope.Object);
        _flexExecutor = new ActionExecutor(mockServiceScopeFactory.Object);
    }

    [Fact]
    public async Task Execute_ReturnsCorrectResult_WhenHandlerExists()
    {
        // Arrange
        var actionType = typeof(MyAction);
        var handlerType = typeof(IActionHandler<MyAction, MyResult>);
        ActionHandlerCache.Cache[(actionType, typeof(IActionHandler<,>))] = handlerType;
        var handler = new MyActionHandler();
        _mockServiceProvider.Setup(sp => sp.GetService(handlerType)).Returns(handler);

        // Act
        var result = await _flexExecutor.Execute<MyResult>(new MyAction());

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
        ActionHandlerCache.Cache[(actionType, customHandlerType)] = handlerType;
        var handler = new CustomActionHandler();
        _mockServiceProvider.Setup(sp => sp.GetService(handlerType)).Returns(handler);

        // Act
        var result = await _flexExecutor.Execute<MyResult>(new MyAction(), customHandlerType);

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
        await _flexExecutor
            .Invoking(async e => 
                await e.Execute<MyResult>(new MyAction(), typeof(IActionHandler<MyAction, MyResult>)))
            .Should().ThrowAsync<IncompatibleActionException>();
    }
}

