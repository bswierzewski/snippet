using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Snippet.Tests.E2E.Core.Auth;
using Snippet.Tests.E2E.Core.Collections;
using Snippet.Tests.E2E.Core.Extensions;
using Snippet.Tests.E2E.Core.Factories;

namespace Snippet.Tests.E2E.Core;

/// <summary>
/// Base class for E2E tests providing common setup, cleanup, and utility methods.
/// Manages test isolation through database resets and service configuration hooks.
/// </summary>
[Collection(nameof(E2ECollection))]
public abstract class TestBase : IAsyncLifetime
{
    // Fields
    private readonly TestWebApplicationFactory _factory;
    private readonly AuthFixture _authFixture;

    // Properties
    protected HttpClient Client { get; private set; } = null!;
    protected IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Override this property to return true if the test requires service customization via OnConfigureServices.
    /// When false (default), the shared factory is used for better performance (~95% faster initialization).
    /// </summary>
    protected virtual bool RequiresServiceCustomization => false;

    // Constructor
    protected TestBase(TestWebApplicationFactory factory, AuthFixture authFixture)
    {
        _factory = factory;
        _authFixture = authFixture;
    }

    // Public methods (IAsyncLifetime)
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabasesAsync();

        // Performance optimization: Only create customized factory when needed.
        // Most tests don't need service customization and can use the shared factory,
        // which avoids re-initializing the application (~10s per test saved).
        var factory = RequiresServiceCustomization
            ? _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(OnConfigureServices))
            : _factory;

        Client = factory.CreateClient();
        Services = factory.Services;

        Client.WithBearerToken(_authFixture.AuthToken);

        await OnInitializeAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    // Protected virtual methods (hooks for derived classes)
    /// <summary>
    /// Override to configure additional or replacement services for the test.
    /// </summary>
    /// <param name="services">The service collection to modify.</param>
    protected virtual void OnConfigureServices(IServiceCollection services) { }

    /// <summary>
    /// Override to perform additional initialization after the test client is created.
    /// </summary>
    protected virtual Task OnInitializeAsync() => Task.CompletedTask;

    // Protected utility methods
    /// <summary>
    /// Creates a new service scope for resolving scoped services.
    /// </summary>
    /// <returns>A disposable service scope.</returns>
    protected IServiceScope CreateScope() => Services.CreateScope();

    /// <summary>
    /// Resolves a required service from the test service provider.
    /// </summary>
    /// <typeparam name="T">The service type to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    protected T Resolve<T>() where T : notnull => Services.GetRequiredService<T>();

    /// <summary>
    /// Registers and returns a mock for the specified service type.
    /// </summary>
    /// <typeparam name="T">The service type to mock.</typeparam>
    /// <param name="services">The service collection to register the mock in.</param>
    /// <param name="behavior">The mock behavior to use.</param>
    /// <returns>The configured mock instance.</returns>
    protected Mock<T> RegisterMock<T>(IServiceCollection services, MockBehavior behavior = MockBehavior.Default) where T : class
        => services.RegisterMock<T>(behavior);
}
