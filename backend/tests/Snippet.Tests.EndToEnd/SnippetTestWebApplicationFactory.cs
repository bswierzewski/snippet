using BuildingBlocks.Modules.Users.Infrastructure.Persistence;
using BuildingBlocks.Tests.EndToEnd.Extensions;
using BuildingBlocks.Tests.EndToEnd.Factories;
using BuildingBlocks.Tests.EndToEnd.Options;
using Microsoft.Extensions.DependencyInjection;
using Snippet.Modules.Snippets.Infrastructure.Persistence;

namespace Snippet.Tests.EndToEnd;

/// <summary>
/// Snippet-specific test web application factory that configures Snippet and BuildingBlocks database contexts.
/// Inherits from the base E2E test factory infrastructure to provide a reusable pattern for other projects.
/// </summary>
public class SnippetTestWebApplicationFactory : TestWebApplicationFactory<Program>
{
    /// <summary>
    /// Specifies DbContext types that need to be migrated during test initialization.
    /// </summary>
    protected override Type[] DbContextTypes =>
    [
        typeof(SnippetsDbContext),
        typeof(UsersDbContext)
    ];

    /// <summary>
    /// Configures the specific database contexts with the test connection string.
    /// </summary>
    protected override void OnConfigureDbContexts(IServiceCollection services, string connectionString)
    {
        services
            .ReplaceDbContext<SnippetsDbContext>(connectionString)
            .ReplaceDbContext<UsersDbContext>(connectionString);
    }

    /// <summary>
    /// Configures authentication options from environment variables.
    /// </summary>
    protected override void OnConfigureServices(IServiceCollection services)
    {
        services.Configure<AuthOptions>(authOptions =>
        {
            authOptions.Provider = Environment.GetEnvironmentVariable("AUTH_PROVIDER") ?? "";

            authOptions.Supabase = new SupabaseAuthOptions
            {
                Url = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "",
                Key = Environment.GetEnvironmentVariable("SUPABASE_KEY") ?? "",
                TestEmail = Environment.GetEnvironmentVariable("SUPABASE_TEST_EMAIL") ?? "",
                TestPassword = Environment.GetEnvironmentVariable("SUPABASE_TEST_PASSWORD") ?? ""
            };

            authOptions.Clerk = new ClerkAuthOptions
            {
                TestToken = Environment.GetEnvironmentVariable("CLERK_TEST_TOKEN") ?? ""
            };
        });
    }
}
