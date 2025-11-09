using System.Runtime.CompilerServices;
using DotNetEnv;

namespace Snippet.Tests.EndToEnd.Core.ModuleInitializers;

/// <summary>
/// Automatically loads environment variables from .env file before any test code executes.
/// Uses ModuleInitializer to guarantee execution order - runs before any fixtures or tests.
/// </summary>
internal static class EnvironmentModuleInitializer
{
    /// <summary>
    /// Loads .env file if it exists. Called automatically by the runtime before module initialization.
    /// This ensures AUTH_PROVIDER and other environment variables are available for AuthFixture.
    /// </summary>
    [ModuleInitializer]
    public static void Initialize()
    {
        if (File.Exists(".env"))
            Env.Load();
    }
}
