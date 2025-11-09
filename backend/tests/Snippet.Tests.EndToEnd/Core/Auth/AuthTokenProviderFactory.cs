using Snippet.Tests.EndToEnd.Core.Auth.Providers;

namespace Snippet.Tests.E2E.Core.Auth;

/// <summary>
/// Factory for creating appropriate auth token provider based on configuration.
/// Uses AUTH_PROVIDER environment variable to determine which provider to use.
/// Supported values: "supabase", "clerk"
/// </summary>
public static class AuthTokenProviderFactory
{
    public static IAuthTokenProvider Create()
    {
        var provider = Environment.GetEnvironmentVariable("AUTH_PROVIDER")?.ToLowerInvariant();

        return provider switch
        {
            "supabase" => new SupabaseAuthTokenProvider(),
            "clerk" => new ClerkAuthTokenProvider(),
            null or "" => throw new InvalidOperationException(
                "AUTH_PROVIDER environment variable is required. Set it to 'supabase' or 'clerk'"),
            _ => throw new InvalidOperationException(
                $"Unknown AUTH_PROVIDER: {provider}. Supported values: 'supabase', 'clerk'")
        };
    }
}
