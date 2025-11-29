using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Authorization;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Modules;
using Shared.Infrastructure.Persistence.Migrations;
using Snippet.Modules.Snippets.Application;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Infrastructure.Endpoints;
using Snippet.Modules.Snippets.Infrastructure.Options;
using Snippet.Modules.Snippets.Infrastructure.Persistence;

namespace Snippet.Modules.Snippets.Infrastructure;

/// <summary>
/// Snippets module - provides code snippet management with collections and tags.
///
/// Features:
/// - Create, read, update, delete snippets
/// - Organize snippets into collections
/// - Tag-based categorization
/// - Favorite snippets
/// - Usage tracking
///
/// Integration:
/// 1. Module is auto-discovered and loaded in AddModules()
/// 2. Endpoints are mapped in Program.cs via extension methods
/// 3. Database migrations run automatically on initialization
/// </summary>
public class SnippetsModule : IModule
{
    /// <summary>
    /// Gets the unique name of the Snippets module
    /// </summary>
    public string Name => "snippets";

    /// <summary>
    /// Register Snippets module services, DbContext, and command/query handlers
    /// </summary>
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register module services using fluent ModuleBuilder API
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<SnippetsDatabaseOptions>(config);
            })
            .AddPostgres<SnippetsDbContext, ISnippetsReadDbContext, ISnippetsWriteDbContext>(sp => sp.GetRequiredService<IOptions<SnippetsDatabaseOptions>>().Value.ConnectionString)
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();
    }

    /// <summary>
    /// Configure middleware pipeline and map endpoints
    /// </summary>
    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        // Map Snippets module endpoints
        endpoints.MapSnippetsEndpoints();
        endpoints.MapCollectionsEndpoints();
        endpoints.MapTagsEndpoints();
        endpoints.MapLookupDataEndpoints();
    }

    /// <summary>
    /// Initializes the Snippets module by running migrations
    /// </summary>
    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<SnippetsDbContext>(serviceProvider).MigrateAsync(cancellationToken);
    }

    /// <summary>
    /// Define permissions available in this module
    /// </summary>
    public IEnumerable<Permission> GetPermissions()
    {
        return
        [
            new Permission("snippets.view", "View snippets", Name, "View snippet content"),
            new Permission("snippets.create", "Create snippets", Name, "Create new snippets"),
            new Permission("snippets.edit", "Edit snippets", Name, "Edit existing snippets"),
            new Permission("snippets.delete", "Delete snippets", Name, "Delete snippets"),
            new Permission("snippets.share", "Share snippets", Name, "Share snippets with others"),
        ];
    }

    /// <summary>
    /// Define roles available in this module
    /// </summary>
    public IEnumerable<Role> GetRoles()
    {
        var permissions = GetPermissions().ToList();

        return
        [
            new Role(
                "snippet-admin",
                "Snippet Administrator",
                Name,
                permissions.AsReadOnly()),

            new Role(
                "snippet-editor",
                "Snippet Editor",
                Name,
                permissions.Where(p => p.Name is not "snippets.delete").ToList().AsReadOnly()),

            new Role(
                "snippet-viewer",
                "Snippet Viewer",
                Name,
                permissions.Where(p => p.Name is "snippets.view").ToList().AsReadOnly())
        ];
    }
}
