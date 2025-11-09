using Snippet.Tests.E2E.Core.Auth;

namespace Snippet.Tests.EndToEnd.Core.Auth.Providers;

/// <summary>
/// Returns a static JWT token from Clerk.
/// Requires environment variable: CLERK_TEST_TOKEN
/// For Clerk, tokens are typically obtained manually through the dashboard or frontend and provided as static values.
/// </summary>
public class ClerkAuthTokenProvider : IAuthTokenProvider
{
    private readonly string _token;

    public ClerkAuthTokenProvider()
    {
        _token = Environment.GetEnvironmentVariable("CLERK_TEST_TOKEN")
            ?? throw new InvalidOperationException("CLERK_TEST_TOKEN environment variable is required");
    }

    public Task<string> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }
}
