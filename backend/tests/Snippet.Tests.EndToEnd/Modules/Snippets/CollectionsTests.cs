using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Tests.Core;
using Shared.Infrastructure.Tests.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;
using Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Tests.EndToEnd.Modules.Snippets;

/// <summary>
/// End-to-end tests for collection management functionality including creation, retrieval, update, and deletion operations.
/// </summary>
[Collection("Snippet")]
public class CollectionsTests(SnippetTestFixture fixture) : IAsyncLifetime
{
    private readonly TestContext _context = fixture.Context;
    private readonly SnippetTestFixture _fixture = fixture;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetCollections_ShouldReturnSuccess()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        var response = await _context.Client.GetAsync("/api/collections");


        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCollection_WithValidData_ShouldCreateAndReturnCollection()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange
        var command = new CreateCollectionCommand(
            "Test Collection",
            "Test Description",
            "#FF5733",
            "📁");

        // Act
        var response = await _context.Client.PostJsonAsync("/api/collections", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database using read context
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var collectionsCount = await readContext.Collections.CountAsync();
        collectionsCount.Should().Be(1);

        var collection = await readContext.Collections.FirstAsync();
        collection.Name.Should().Be("Test Collection");
        collection.Description.Should().Be("Test Description");
        collection.Color.Should().Be("#FF5733");
        collection.Icon.Should().Be("📁");
    }

    [Fact]
    public async Task GetCollections_WithMultipleCollections_ShouldReturnCorrectCount()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange - Create multiple collections
        var commands = new[]
        {
            new CreateCollectionCommand("Collection 1", "Desc 1", "#FF0000", "📁"),
            new CreateCollectionCommand("Collection 2", "Desc 2", "#00FF00", "📂"),
            new CreateCollectionCommand("Collection 3", "Desc 3", "#0000FF", "📚")
        };

        foreach (var command in commands)
        {
            await _context.Client.PostJsonAsync("/api/collections", command);
        }

        // Act
        var response = await _context.Client.GetAsync("/api/collections");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify database count using read context
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var collectionsCount = await readContext.Collections.CountAsync();
        collectionsCount.Should().Be(3);
    }

    [Fact]
    public async Task GetCollectionById_WithExistingId_ShouldReturnCollection()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange
        var createResponse = await _context.Client.PostJsonAsync("/api/collections", new CreateCollectionCommand(
            "Test Collection",
            "Test Description",
            "#FF5733",
            "📁"
        ));

        var collectionId = await createResponse.ReadAsJsonAsync<Guid>();

        // Act
        var response = await _context.Client.GetAsync($"/api/collections/{collectionId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify collection exists in database using read context
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var collection = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(collectionId));
        collection.Should().NotBeNull();
        collection!.Name.Should().Be("Test Collection");
    }

    [Fact]
    public async Task UpdateCollection_WithValidData_ShouldUpdateCollection()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await _context.Client.PostJsonAsync("/api/collections", new CreateCollectionCommand(
            "Original Name",
            "Original Description",
            "#000000",
            "📁"
        ));

        var collectionId = await createResponse.ReadAsJsonAsync<Guid>();

        var updateCommand = new UpdateCollectionCommand(
            collectionId,
            "Updated Name",
            "Updated Description",
            "#FFFFFF",
            "📂"
        );

        // Act
        var response = await _context.Client.PutJsonAsync($"/api/collections/{collectionId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update in database
        var updatedCollection = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(collectionId));
        updatedCollection.Should().NotBeNull();
        updatedCollection!.Name.Should().Be("Updated Name");
        updatedCollection.Description.Should().Be("Updated Description");
        updatedCollection.Color.Should().Be("#FFFFFF");
        updatedCollection.Icon.Should().Be("📂");
    }

    [Fact]
    public async Task DeleteCollection_WithExistingId_ShouldRemoveFromDatabase()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await _context.Client.PostJsonAsync("/api/collections", new CreateCollectionCommand(
            "To Delete Collection",
            "Will be deleted",
            "#FF0000",
            "🗑️"
        ));

        var collectionId = await createResponse.ReadAsJsonAsync<Guid>();

        // Verify collection exists before deletion
        var collectionBeforeDelete = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(collectionId));
        collectionBeforeDelete.Should().NotBeNull();

        // Act - Delete via HTTP
        var response = await _context.Client.DeleteAsync($"/api/collections/{collectionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify collection no longer exists
        var collectionAfterDelete = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(collectionId));
        collectionAfterDelete.Should().BeNull();

        var totalCount = await readContext.Collections.CountAsync();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetCollectionById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _context.Client.GetAsync($"/api/collections/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCollection_WithNonExistingId_ShouldReturnNotFound()
    {
        // Setup
        await _context.ResetDatabaseAsync();
        var token = await _context.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);

        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _context.Client.DeleteAsync($"/api/collections/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
