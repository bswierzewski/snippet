using BuildingBlocks.Abstractions.Abstractions;

namespace Snippet.Modules.Snippets.Application;

/// <summary>
/// Marker class for the Users module Application assembly.
/// Enables automatic discovery and registration of:
/// - MediatR handlers (commands, queries, notifications)
/// - FluentValidation validators
/// </summary>
public sealed class ApplicationAssembly : IModuleAssembly
{
}
