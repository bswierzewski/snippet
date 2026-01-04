using BuildingBlocks.Infrastructure.Extensions;
using DotNetEnv;
using Snippet.Modules.Snippets.Infrastructure;

if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddCors();

builder.Services.AddProblemDetails(options =>
    options.AddCustomConfiguration(builder.Environment));

builder.Services.AddOpenApi(options =>
    options.AddProblemDetailsSchemas());

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

builder.Services.AddUserContext();

builder.Services.RegisterModules(builder.Configuration, [new SnippetsModule()]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
}

app.UseAuthentication();
app.UseAuthorization();

app.UseModules(builder.Configuration);

await app.Services.InitModules();
await app.RunAsync();