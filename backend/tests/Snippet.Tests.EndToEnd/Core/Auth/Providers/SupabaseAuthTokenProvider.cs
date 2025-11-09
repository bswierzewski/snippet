using Snippet.Tests.E2E.Core.Auth;
using Supabase;

namespace Snippet.Tests.EndToEnd.Core.Auth.Providers;

/// <summary>
/// Fetches JWT tokens from Supabase by authenticating with email/password.
/// Requires environment variables: SUPABASE_URL, SUPABASE_KEY, SUPABASE_TEST_EMAIL, SUPABASE_TEST_PASSWORD
/// </summary>
public class SupabaseAuthTokenProvider : IAuthTokenProvider
{
    private readonly string _url;
    private readonly string _key;
    private readonly string _email;
    private readonly string _password;

    public SupabaseAuthTokenProvider()
    {
        _url = Environment.GetEnvironmentVariable("SUPABASE_URL")
            ?? throw new InvalidOperationException("SUPABASE_URL environment variable is required");

        _key = Environment.GetEnvironmentVariable("SUPABASE_KEY")
            ?? throw new InvalidOperationException("SUPABASE_KEY environment variable is required");

        _email = Environment.GetEnvironmentVariable("SUPABASE_TEST_EMAIL")
            ?? throw new InvalidOperationException("SUPABASE_TEST_EMAIL environment variable is required");

        _password = Environment.GetEnvironmentVariable("SUPABASE_TEST_PASSWORD")
            ?? throw new InvalidOperationException("SUPABASE_TEST_PASSWORD environment variable is required");
    }

    public async Task<string> GetTokenAsync()
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = false,
            AutoConnectRealtime = false
        };

        var client = new Client(_url, _key, options);
        await client.InitializeAsync();

        var session = await client.Auth.SignIn(_email, _password);

        if (session?.AccessToken == null)
        {
            throw new InvalidOperationException("Failed to authenticate with Supabase. Check credentials.");
        }

        return session.AccessToken;
    }
}
