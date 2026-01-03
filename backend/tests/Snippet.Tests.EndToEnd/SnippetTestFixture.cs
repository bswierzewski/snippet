using BuildingBlocks.Abstractions.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Tests.Core;
using BuildingBlocks.Tests.Infrastructure.Containers;
using Snippet.Tests.EndToEnd.Mocks;

namespace Snippet.Tests.EndToEnd;

/// <summary>
/// Shared test fixture for Snippet module end-to-end tests.
/// Provides shared infrastructure (PostgreSQL container, TestContext) across all test classes.
/// </summary>
/// <remarks>
/// This fixture is created ONCE per test collection and shared across all test classes.
///
/// It provides:
/// - PostgreSQL container (started once, shared)
/// - TestContext (shared across all tests)
/// - Mocked user context for authentication
/// </remarks>
public class SnippetTestFixture : IAsyncLifetime
{
    /// <summary>
    /// Gets the shared PostgreSQL container.
    /// </summary>
    public PostgreSqlTestContainer Container { get; } = new();

    /// <summary>
    /// Gets the shared test context.
    /// All tests in the collection use this same context.
    /// </summary>
    public TestContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Start PostgreSQL container (once for all tests)
        await Container.StartAsync();

        // Create shared test context
        Context = await TestContext.CreateBuilder<Program>()
            .WithContainer(Container)
            .WithServices((services, configuration) =>
            {
                // Replace real authentication with mock for testing
                services.AddAuthentication(MockAuthenticationHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>(
                        MockAuthenticationHandler.SchemeName,
                        options => { });

                // Replace real user context with mock for testing
                services.AddScoped<IUserContext, MockUserContext>();
            })
            .BuildAsync();
    }

    public async Task DisposeAsync()
    {
        if (Context != null)
        {
            await Context.DisposeAsync();
        }

        await Container.StopAsync();
    }
}

/// <summary>
/// xUnit collection definition for sharing the SnippetTestFixture across tests.
/// All tests with [Collection("Snippet")] share a single PostgreSQL container and TestContext.
/// </summary>
[CollectionDefinition("Snippet")]
public class SnippetCollection : ICollectionFixture<SnippetTestFixture>
{
}
