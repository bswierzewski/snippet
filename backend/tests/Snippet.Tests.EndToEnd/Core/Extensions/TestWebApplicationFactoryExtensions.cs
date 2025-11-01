using Snippet.Tests.E2E.Core.Factories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Snippet.Tests.E2E.Core.Extensions;

/// <summary>
/// Extensions for TestWebApplicationFactory to simplify test client creation with custom service configurations.
/// </summary>
public static class TestWebApplicationFactoryExtensions
{
    /// <summary>
    /// Creates an HTTP client with custom service configuration for testing specific scenarios.
    /// </summary>
    /// <param name="factory">The test web application factory instance.</param>
    /// <param name="configureServices">Action to configure additional or replacement services.</param>
    /// <returns>An HTTP client configured with the custom services.</returns>
    public static HttpClient CreateClientWithServices(this TestWebApplicationFactory factory, Action<IServiceCollection> configureServices)
    {
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(configureServices);
        });

        return customFactory.CreateClient();
    }
}
