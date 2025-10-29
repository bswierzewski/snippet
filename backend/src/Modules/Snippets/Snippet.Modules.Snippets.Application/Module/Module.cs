using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Entities;

namespace Snippet.Modules.Snippets.Application.Module;

public class Module : IModule
{
    public string ModuleName => "Snippets";

    public string DisplayName => "Snippets Manager";

    public string? Description => "Module for managing code snippets with tags and quick copy functionality";

    public IEnumerable<Permission> GetPermissions()
    {
        return [];
    }

    public IEnumerable<Role> GetRoles()
    {
        return [];
    }
}
