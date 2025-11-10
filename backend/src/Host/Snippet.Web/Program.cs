using BuildingBlocks.Modules.Users.Web;
using BuildingBlocks.Modules.Users.Web.Endpoints;
using BuildingBlocks.Modules.Users.Web.Extensions;
using BuildingBlocks.Modules.Users.Web.Extensions.JwtBearers;
using DotNetEnv;
using Snippet.Modules.Snippets.Infrastructure;
using Snippet.Web.Endpoints;

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

// Add Modules
builder.Services.AddUsers(builder.Configuration);
builder.Services.AddSnippets(builder.Configuration);

// Configure Supabase authentication
builder.Services.AddSupabaseOptions(builder.Configuration);
builder.Services.AddAuthentication().AddSupabaseJwtBearer();
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
}

// Middleware pipeline order matters!
app.UseAuthentication(); // 1. Authentication first
app.UseAuthorization();  // 2. Authorization second

// Health check endpoint (no authentication required)
app.MapHealthChecks("/api/health");

// Map endpoints from modules
app.MapUsersEndpoints();
app.MapCollectionsEndpoints();
app.MapSnippetsEndpoints();
app.MapTagsEndpoints();
app.MapLookupDataEndpoints();

app.Run();

// Make the Program class accessible for integration tests
public partial class Program { }