using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snippet.Modules.Snippets.Application.Module;
using Snippet.Modules.Snippets.Infrastructure.Module;

namespace Snippet.Modules.Snippets.Infrastructure;

/// <summary>
/// Main entry point for registering the Snippets module.
/// Orchestrates registration of Application and Infrastructure layers.
/// </summary>
public static class ModuleRegistration
{
    /// <summary>
    /// Registers all services for the Snippets module (Application + Infrastructure layers).
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <param name="configuration">Application configuration for connection strings and settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSnippets(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Application layer (MediatR, validators, module)
        services.AddApplication();

        // Register Infrastructure layer (DbContext, repositories, interceptors)
        services.AddInfrastructure(configuration);

        return services;
    }
}
