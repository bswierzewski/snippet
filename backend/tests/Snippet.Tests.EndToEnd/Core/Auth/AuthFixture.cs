namespace Snippet.Tests.E2E.Core.Auth;

/// <summary>
/// Collection fixture that provides a shared authentication token for all E2E tests.
/// The token is initialized once per test collection, reducing authentication overhead.
/// </summary>
public class AuthFixture : IAsyncLifetime
{
    /// <summary>
    /// Gets the authentication token to be used in HTTP requests.
    /// </summary>
    public string AuthToken { get; private set; } = null!;

    /// <summary>
    /// Initializes the fixture by obtaining an authentication token from the configured provider.
    /// </summary>
    public async Task InitializeAsync()
    {
        var tokenProvider = AuthTokenProviderFactory.Create();
        AuthToken = await tokenProvider.GetTokenAsync();
    }

    /// <summary>
    /// Performs cleanup when the test collection is finished.
    /// </summary>
    public Task DisposeAsync() => Task.CompletedTask;
}
