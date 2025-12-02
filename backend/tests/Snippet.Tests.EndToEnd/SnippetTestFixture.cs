using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Tests.Authentication;
using Shared.Infrastructure.Tests.Core;
using Shared.Infrastructure.Tests.Infrastructure.Containers;

namespace Snippet.Tests.EndToEnd;

/// <summary>
/// Shared test fixture for Snippet module end-to-end tests.
/// Provides shared infrastructure (PostgreSQL container, TestContext) across all test classes.
/// </summary>
/// <remarks>
/// This fixture is created ONCE per test collection and shared across all test classes.
/// Since Snippet module tests don't use mocks, they share a single TestContext.
/// 
/// It provides:
/// - PostgreSQL container (started once, shared)
/// - TestContext (shared across all tests)
/// - Token provider with built-in cache
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

    /// <summary>
    /// Gets the test user options (email, password) from configuration.
    /// </summary>
    public TestUserOptions TestUser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Start PostgreSQL container (once for all tests)
        await Container.StartAsync();

        // Create shared test context
        Context = await TestContext.CreateBuilder<Program>()
            .WithContainer(Container)
            .WithServices((services, configuration) =>
            {
                // Register test user credentials from appsettings
                services.ConfigureOptions<TestUserOptions>(configuration);

                // Register Supabase token provider for authentication
                services.AddSingleton<ITokenProvider, SupabaseTokenProvider>();
            })
            .BuildAsync();

        // Get test user configuration
        TestUser = Context.GetRequiredService<IOptions<TestUserOptions>>().Value;
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
