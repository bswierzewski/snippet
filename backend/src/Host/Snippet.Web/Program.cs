using DotNetEnv;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Modules;
using Shared.Users.Infrastructure.Extensions.JwtBearers;
using Snippet.Modules.Snippets.Infrastructure.Persistence;

// Load environment variables from .env file BEFORE creating builder
// clobberExistingVars: false ensures Docker/CI/CD environment variables take precedence
if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register core services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();

// Health checks for Docker and Caddy monitoring
builder.Services.AddHealthChecks();

// OpenAPI for Orval client generation
builder.Services.AddEndpointsApiExplorer(); // Exposes Minimal API endpoints to OpenAPI
builder.Services.AddOpenApi();              // Generates OpenAPI document

// Load and register all modules
// Auto-discovers IModule implementations from all loaded assemblies
var modules = ModuleLoader.LoadModules();

builder.Services.AddSingleton<IReadOnlyCollection<IModule>>(modules.AsReadOnly());

builder.Services.RegisterModules(modules, builder.Configuration);

// Configure authentication - JWT from Users module
builder.Services.AddAuthentication()
    .AddTestJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Generates OpenAPI JSON at /openapi/v1.json

    // CORS only in Development (production runs in single Docker container)
    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

    // Seed database in development
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SnippetsDbContext>();
    var seeder = new DataSeeder(dbContext);
    await seeder.SeedAsync();
}

// Middleware pipeline order matters!
app.UseAuthentication(); // 1. Authentication first
app.UseAuthorization();  // 2. Authorization second

// Health check endpoint (no authentication required)
app.MapHealthChecks("/api/health");

// Configure modules middleware pipeline
// Modules configure their own middleware and endpoints
app.UseModules(modules, builder.Configuration);

// Initialize all modules (run migrations, seed data, etc.)
await app.Services.InitializeModules(modules);

await app.RunAsync();

// Make the Program class accessible for integration tests
public partial class Program { }