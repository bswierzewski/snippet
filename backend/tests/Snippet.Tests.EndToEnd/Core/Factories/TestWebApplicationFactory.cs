using Snippet.Modules.Snippets.Infrastructure.Persistence;
using Snippet.Tests.E2E.Core.Extensions;
using BuildingBlocks.Modules.Users.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Snippet.Tests.E2E.Core.Factories;

/// <summary>
/// Test web application factory that manages PostgreSQL test containers and database setup for E2E tests.
/// Provides isolated test databases with automatic cleanup and reset capabilities.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithDatabase("postgres")
            .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _connection = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _connection = new NpgsqlConnection(_container.GetConnectionString());
        await _connection.OpenAsync();

        using (var scope = Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            await serviceProvider.GetRequiredService<SnippetsDbContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<UsersDbContext>().Database.MigrateAsync();
        }

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"],
            WithReseed = true
        });
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Resets all test databases to a clean state by truncating tables while preserving schema.
    /// </summary>
    public async Task ResetDatabasesAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var connectionString = _container.GetConnectionString();

            services
                .ReplaceDbContext<SnippetsDbContext>(connectionString)
                .ReplaceDbContext<UsersDbContext>(connectionString);
        });
    }
}
