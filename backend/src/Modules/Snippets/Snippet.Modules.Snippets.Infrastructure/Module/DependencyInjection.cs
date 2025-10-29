using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Infrastructure.Persistence;

namespace Snippet.Modules.Snippets.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register migration service for automatic database migrations
        services.AddMigrationService<SnippetsDbContext>();

        // Register EF Core interceptors for cross-cutting concerns
        services
            .AddAuditableEntityInterceptor()      // Automatically populates CreatedAt, ModifiedAt fields
            .AddDomainEventDispatchInterceptor(); // Publishes domain events via MediatR

        // Register DbContext with PostgreSQL and interceptors
        services.AddDbContext<SnippetsDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("SnippetsConnection")
                ?? throw new InvalidOperationException("SnippetsConnection string is not configured");

            options.UseNpgsql(connectionString)
                   .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        // Register read/write context abstractions (CQRS pattern)
        services.AddScoped<ISnippetsWriteDbContext>(provider => provider.GetRequiredService<SnippetsDbContext>());
        services.AddScoped<ISnippetsReadDbContext>(provider => provider.GetRequiredService<SnippetsDbContext>());

        return services;
    }
}
