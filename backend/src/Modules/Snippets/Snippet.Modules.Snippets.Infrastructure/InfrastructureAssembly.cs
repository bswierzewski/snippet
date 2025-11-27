using Shared.Abstractions.Modules;

namespace Snippet.Modules.Snippets.Infrastructure;

/// <summary>
/// Marker class for the Users module Infrastructure assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// - Module endpoints (IModuleEndpoints)
/// </summary>
public sealed class InfrastructureAssembly : IModuleAssembly
{
}
