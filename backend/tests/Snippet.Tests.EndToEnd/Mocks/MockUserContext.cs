using BuildingBlocks.Abstractions.Abstractions;

namespace Snippet.Tests.EndToEnd.Mocks;

/// <summary>
/// Mock implementation of IUserContext for testing purposes.
/// Returns a fixed test user ID without requiring authentication.
/// </summary>
public class MockUserContext : IUserContext
{
    public Guid Id => Guid.Parse("00000000-0000-0000-0000-000000000001");

    public IEnumerable<string> Roles => ["user", "admin"];

    public bool IsInRole(string role) => Roles.Contains(role);
}
