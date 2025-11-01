using Snippet.Tests.E2E.Core.Factories;
using Xunit;

namespace Snippet.Tests.E2E.Core.Collections
{
    /// <summary>
    /// Defines a shared test collection that ensures all E2E tests run sequentially using the same test web application factory.
    /// </summary>
    [CollectionDefinition(nameof(E2ECollection))]
    public class E2ECollection : ICollectionFixture<TestWebApplicationFactory>
    {
    }
}
