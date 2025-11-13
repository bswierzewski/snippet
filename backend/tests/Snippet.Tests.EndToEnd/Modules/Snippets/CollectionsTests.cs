using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;
using Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;
using Snippet.Modules.Snippets.Domain.ValueObjects;
using BuildingBlocks.Tests.EndToEnd;
using BuildingBlocks.Tests.EndToEnd.Auth;
using BuildingBlocks.Tests.EndToEnd.Extensions;

namespace Snippet.Tests.EndToEnd.Modules.Snippets;

/// <summary>
/// End-to-end tests for collection management functionality including creation, retrieval, update, and deletion operations.
/// </summary>
[Collection(nameof(SnippetE2ECollection))]
public class CollectionsTests(SnippetTestWebApplicationFactory factory, AuthFixture authFixture) : SnippetTestBase(factory, authFixture)
{  
    [Fact]
    public async Task GetCollections_ShouldReturnSuccess()
    {
        var response = await Client.GetAsync("/api/collections");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateCollection_WithValidData_ShouldCreateAndReturnCollection()
    {
        // Arrange
        var command = new CreateCollectionCommand(
            "Test Collection",
            "Test Description",
            "#FF5733",
            "📁");

        // Act
        var response = await Client.PostJsonAsync("/api/collections", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
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
        // Arrange - Create multiple collections
        var commands = new[]
        {
            new CreateCollectionCommand("Collection 1", "Desc 1", "#FF0000", "📁"),
            new CreateCollectionCommand("Collection 2", "Desc 2", "#00FF00", "📂"),
            new CreateCollectionCommand("Collection 3", "Desc 3", "#0000FF", "📚")
        };

        foreach (var command in commands)
        {
            await Client.PostJsonAsync("/api/collections", command);
        }

        // Act
        var response = await Client.GetAsync("/api/collections");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify database count using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var collectionsCount = await readContext.Collections.CountAsync();
        collectionsCount.Should().Be(3);
    }

    [Fact]
    public async Task GetCollectionById_WithExistingId_ShouldReturnCollection()
    {
        // Arrange
        var createResponse = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand(
            "Test Collection",
            "Test Description",
            "#FF5733",
            "📁"
        ));

        var collectionId = await createResponse.ReadAsJsonAsync<Guid>();

        // Act
        var response = await Client.GetAsync($"/api/collections/{collectionId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify collection exists in database using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var collection = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(collectionId));
        collection.Should().NotBeNull();
        collection!.Name.Should().Be("Test Collection");
    }

    [Fact]
    public async Task UpdateCollection_WithValidData_ShouldUpdateCollection()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand(
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
        var response = await Client.PutJsonAsync($"/api/collections/{collectionId}", updateCommand);

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
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand(
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
        var response = await Client.DeleteAsync($"/api/collections/{collectionId}");

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
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/collections/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCollection_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/collections/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
