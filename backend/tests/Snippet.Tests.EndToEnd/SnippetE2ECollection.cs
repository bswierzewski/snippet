using BuildingBlocks.Tests.EndToEnd;
using BuildingBlocks.Tests.EndToEnd.Auth;

namespace Snippet.Tests.EndToEnd;

/// <summary>
/// Snippet-specific E2E test collection configuration.
/// Ensures all Snippet E2E tests run sequentially using the Snippet-configured test factory.
/// </summary>
[CollectionDefinition(nameof(SnippetE2ECollection))]
public class SnippetE2ECollection :
    ICollectionFixture<SnippetTestWebApplicationFactory>,
    ICollectionFixture<AuthFixture>
{
}

/// <summary>
/// Base class for Snippet E2E tests with pre-configured factory and auth fixture.
/// </summary>
public abstract class SnippetTestBase : TestBase
{
    protected SnippetTestBase(SnippetTestWebApplicationFactory factory, AuthFixture authFixture)
        : base(factory, authFixture)
    {
    }
}
