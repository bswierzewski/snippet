using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Snippet.Modules.Snippets.Application;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain;
using Snippet.Modules.Snippets.Infrastructure.Endpoints;
using Snippet.Modules.Snippets.Infrastructure.Options;
using Snippet.Modules.Snippets.Infrastructure.Persistence;

namespace Snippet.Modules.Snippets.Infrastructure;

public class SnippetsModule : IModule
{
    public string Name => Module.Name;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<SnippetsDatabaseOptions>(config);
            })
            .AddPostgres<SnippetsDbContext, ISnippetDbContext>(sp => sp.GetRequiredService<IOptions<SnippetsDatabaseOptions>>().Value.ConnectionString)
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();
    }

    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        endpoints.MapSnippetsEndpoints();
        endpoints.MapCollectionsEndpoints();
        endpoints.MapTagsEndpoints();
        endpoints.MapLookupDataEndpoints();
    }

    public async Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await new MigrationService<SnippetsDbContext>(serviceProvider).MigrateAsync(cancellationToken);
    }
}
