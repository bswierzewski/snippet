using BuildingBlocks.Modules.Users.Infrastructure.Persistence;
using BuildingBlocks.Tests.EndToEnd.Extensions;
using BuildingBlocks.Tests.EndToEnd.Factories;
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
}
