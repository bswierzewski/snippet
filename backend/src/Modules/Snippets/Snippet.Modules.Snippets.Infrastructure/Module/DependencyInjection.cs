using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Infrastructure.Options;
using Snippet.Modules.Snippets.Infrastructure.Persistence;

namespace Snippet.Modules.Snippets.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register database options from configuration
        services.Configure<SnippetsDatabaseOptions>(
            configuration.GetSection(SnippetsDatabaseOptions.SectionName));

        // Register migration service for automatic database migrations
        services.AddMigrationService<SnippetsDbContext>();

        // Register EF Core interceptors for cross-cutting concerns
        services
            .AddAuditableEntityInterceptor()      // Automatically populates CreatedAt, ModifiedAt fields
            .AddDomainEventDispatchInterceptor(); // Publishes domain events via MediatR

        // Register DbContext with PostgreSQL and interceptors
        services.AddDbContext<SnippetsDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<SnippetsDatabaseOptions>>().Value;

            options.UseNpgsql(dbOptions.ConnectionString)
                   .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        // Register read/write context abstractions (CQRS pattern)
        services.AddScoped<ISnippetsWriteDbContext>(provider => provider.GetRequiredService<SnippetsDbContext>());
        services.AddScoped<ISnippetsReadDbContext>(provider => provider.GetRequiredService<SnippetsDbContext>());

        return services;
    }
}
