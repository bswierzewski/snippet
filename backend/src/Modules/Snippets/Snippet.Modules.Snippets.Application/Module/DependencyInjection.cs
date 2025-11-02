using System.Reflection;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Snippet.Modules.Snippets.Application.Module;

/// <summary>
/// Provides dependency injection configuration for the Snippets application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application layer services including MediatR, validators, and the Products API.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    /// <returns>The configured service collection for method chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR with behaviors pipeline
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddLoggingBehavior()
               .AddUnhandledExceptionBehavior()
               .AddValidationBehavior()
               .AddAuthorizationBehavior()
               .AddPerformanceMonitoringBehavior();
        });

        // Register FluentValidation validators
        services.AddValidators();

        // Register module singleton for discovery
        services.AddSingleton<IModule, Module>();

        return services;
    }
}
