namespace Snippet.Tests.E2E.Core;

/// <summary>
/// Contains JWT tokens for E2E testing with different user roles and permissions.
/// These tokens are issued by authentication providers (Clerk, Supabase) for testing purposes and have extended expiration dates.
/// </summary>
public static class TestJwtTokens
{
    /// <summary>
    /// JWT token from Clerk for a test user with admin/manager permissions across all modules.
    /// User: swierzewski.bartosz@gmail.com
    /// Subject: user_2wYRiRPEB1wuCn6XhnBCJz8EBjJ
    /// Issuer: https://fitting-wasp-30.clerk.accounts.dev
    /// Algorithm: RS256
    /// Valid until: 2026-10-14
    /// </summary>
    public const string ClerkToken = "eyJhbGciOiJSUzI1NiIsImNhdCI6ImNsX0I3ZDRQRDIyMkFBQSIsImtpZCI6Imluc18yd1lQZ1ROcGJJNjB0UUI5NHBlZFB2Q2RKOGgiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOiJ5b3VyLWFwaS1pZGVudGlmaWVyIiwiYXpwIjoiaHR0cDovL2xvY2FsaG9zdDozMDAwIiwiZW1haWwiOiJzd2llcnpld3NraS5iYXJ0b3N6QGdtYWlsLmNvbSIsImV4cCI6MTc5MjEyNzU2MiwiaWF0IjoxNzYwNTkxNTYyLCJpc3MiOiJodHRwczovL2ZpdHRpbmctd2FzcC0zMC5jbGVyay5hY2NvdW50cy5kZXYiLCJqdGkiOiIzNTg1MWQ3MjM5ZjljYzFkOTMzZSIsIm5hbWUiOiJCYXJ0b3N6IMWad2llcnpld3NraSIsIm5iZiI6MTc2MDU5MTU1NywicGljdHVyZSI6Imh0dHBzOi8vaW1nLmNsZXJrLmNvbS9leUowZVhCbElqb2ljSEp2ZUhraUxDSnpjbU1pT2lKb2RIUndjem92TDJsdFlXZGxjeTVqYkdWeWF5NWtaWFl2ZFhCc2IyRmtaV1F2YVcxblh6SjNiVXN3TjB0ME1VRmxjamxHZEU5elprMDFjR3B3WlRGRVdTSjkiLCJzdWIiOiJ1c2VyXzJ3WVJpUlBFQjF3dUNuNlhobkJDSno4RUJqSiJ9.XRRlVarEg8GwdRPDnurOWQ4GQq83jBfrnw3WKBLcnjFNcyHtoq9u9pIsMI4gwBlkPCthgY9N_AbDGTL842p833ffBKS7bcsy3jYZZ0bDakOiCOTZqcztKeMWcEv8CLJ9M2wGP_kAYA7MUC8MV2UrgYgpHRv60p8cg41I9Wctz0_yC6Xcv0_bWWakcgkqIeSJxk4i5Zh1eYdFYXqlIfaAje-7VH-bITIp4vjb7fKXSwqFTeQo5SsVD3sqybbd9RBjW2b9WcUTpPCBy7u68TsgNM_3BXKj2QWJwEoTwalBssyshFUSjkx_x4-MxBfTmFIOnR3wdi9jEjMSGIg-H3YuPA";

    /// <summary>
    /// JWT token from Supabase for the same test user.
    /// User: swierzewski.bartosz@gmail.com
    /// Subject: 3903cf3e-4069-49a5-bea5-d5abc3e908d5
    /// Issuer: https://mapwpeemcvaexekckhfm.supabase.co/auth/v1
    /// Algorithm: ES256
    /// Valid until: Long-lived for testing purposes
    ///
    /// IMPORTANT: To generate a new long-lived Supabase token for testing:
    /// 1. Log in to your Supabase project
    /// 2. Use the Supabase client to authenticate and get a token
    /// 3. Optionally use Supabase admin API to create custom JWTs with extended expiration
    /// 4. Replace this placeholder with your long-lived token
    /// </summary>
    public const string SupabaseToken = "eyJhbGciOiJFUzI1NiIsImtpZCI6IjQyNjc5Yzk1LWRmY2MtNDBhYy04MWYxLTA4NGIwZGViZDZkMiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL21hcHdwZWVtY3ZhZXhla2NraGZtLnN1cGFiYXNlLmNvL2F1dGgvdjEiLCJzdWIiOiIzOTAzY2YzZS00MDY5LTQ5YTUtYmVhNS1kNWFiYzNlOTA4ZDUiLCJhdWQiOiJhdXRoZW50aWNhdGVkIiwiZXhwIjoxNzYyMzI4NTAzLCJpYXQiOjE3NjIzMjQ5MDMsImVtYWlsIjoic3dpZXJ6ZXdza2kuYmFydG9zekBnbWFpbC5jb20iLCJwaG9uZSI6IiIsImFwcF9tZXRhZGF0YSI6eyJwcm92aWRlciI6ImVtYWlsIiwicHJvdmlkZXJzIjpbImVtYWlsIl19LCJ1c2VyX21ldGFkYXRhIjp7ImVtYWlsIjoic3dpZXJ6ZXdza2kuYmFydG9zekBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwicGhvbmVfdmVyaWZpZWQiOmZhbHNlLCJzdWIiOiIzOTAzY2YzZS00MDY5LTQ5YTUtYmVhNS1kNWFiYzNlOTA4ZDUifSwicm9sZSI6ImF1dGhlbnRpY2F0ZWQiLCJhYWwiOiJhYWwxIiwiYW1yIjpbeyJtZXRob2QiOiJwYXNzd29yZCIsInRpbWVzdGFtcCI6MTc2MjMyNDkwM31dLCJzZXNzaW9uX2lkIjoiNzk1MGFjODgtZDU4My00NDcxLWFmYzktZDA0NWFlYWQ4MGNmIiwiaXNfYW5vbnltb3VzIjpmYWxzZX0.iy3TpkaHYYhlYngyJWRkkblp9rZLHFd31ju8q8ayG3GzLSJaqqShJtetzj2D-ZlaEctlEeuXl7fNoPlJlzh20A";

    /// <summary>
    /// Default token to use in tests. Currently uses Clerk token for backwards compatibility.
    /// Can be switched to SupabaseToken once a long-lived token is generated.
    /// </summary>
    public const string Default = SupabaseToken;
}
