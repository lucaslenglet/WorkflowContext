using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WorkflowContext.Tests;

public class WorkflowContextFactoryTests
{
    [Fact]
    public void AddWorkflowContext_RegistersAScopedFactory()
    {
        using var provider = new ServiceCollection()
            .AddWorkflowContext()
            .BuildServiceProvider();
        using var scope = provider.CreateScope();

        var factory = scope.ServiceProvider.GetRequiredService<IWorkflowContextFactory>();

        Assert.Same(factory, scope.ServiceProvider.GetRequiredService<IWorkflowContextFactory>());
    }

    [Fact]
    public void Create_BindsTheContextToTheScope_WithTheGivenDataAndToken()
    {
        using var provider = new ServiceCollection()
            .AddWorkflowContext()
            .BuildServiceProvider();
        using var scope = provider.CreateScope();
        using var cancellation = new CancellationTokenSource();
        var data = new TestData();

        var context = scope.ServiceProvider
            .GetRequiredService<IWorkflowContextFactory>()
            .Create<TestData, TestError>(data, cancellation.Token);

        Assert.Same(scope.ServiceProvider, context.Services);
        Assert.Same(data, context.Data);
        Assert.Equal(cancellation.Token, context.CancellationToken);
        Assert.True(context.State.IsSuccess);
    }

    [Fact]
    public void Create_WithoutData_CreatesANewInstance()
    {
        using var provider = new ServiceCollection()
            .AddWorkflowContext()
            .BuildServiceProvider();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<IWorkflowContextFactory>()
            .Create<TestData, TestError>();

        Assert.NotNull(context.Data);
        Assert.Equal(CancellationToken.None, context.CancellationToken);
    }

    [Fact]
    public void Deconstruct_ReturnsDataStateAndServices()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var data = new TestData();

        var (deconstructedData, state, deconstructedServices) = new WorkflowContext<TestData, TestError>(services, data);

        Assert.Same(data, deconstructedData);
        Assert.True(state.IsSuccess);
        Assert.Same(services, deconstructedServices);
    }
}
