using BuildingBlocks.Tests.EndToEnd;

namespace Snippet.Tests.EndToEnd;

/// <summary>
/// Snippet-specific E2E test collection configuration.
/// Ensures all Snippet E2E tests run sequentially using the Snippet-configured test factory.
/// </summary>
[CollectionDefinition(nameof(SnippetE2ECollection))]
public class SnippetE2ECollection : ICollectionFixture<SnippetTestWebApplicationFactory>
{
}

/// <summary>
/// Base class for Snippet E2E tests with pre-configured factory.
/// Individual test classes can override OnInitializeAsync to configure authentication.
/// </summary>
public abstract class SnippetTestBase(SnippetTestWebApplicationFactory factory) : TestBase(factory)
{
}
