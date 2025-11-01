using System.Net;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;
using Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;
using Snippet.Tests.E2E.Core;
using Snippet.Tests.E2E.Core.Extensions;
using Snippet.Tests.E2E.Core.Factories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Snippet.Tests.E2E.Modules.Snippets;

/// <summary>
/// End-to-end tests for collection management functionality including creation, retrieval, update, and deletion operations.
/// </summary>
public class CollectionsTests(TestWebApplicationFactory factory) : TestBase(factory)
{
    protected override Task OnInitializeAsync()
    {
        Client.WithBearerToken(TestJwtTokens.Default);
        return Task.CompletedTask;
    }

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
        var mediator = Services.GetRequiredService<IMediator>();

        var commands = new[]
        {
            new CreateCollectionCommand("Collection 1", "Desc 1", "#FF0000", "📁"),
            new CreateCollectionCommand("Collection 2", "Desc 2", "#00FF00", "📂"),
            new CreateCollectionCommand("Collection 3", "Desc 3", "#0000FF", "📚")
        };

        foreach (var command in commands)
        {
            await mediator.Send(command);
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
        var mediator = Services.GetRequiredService<IMediator>();

        var result = await mediator.Send(new CreateCollectionCommand(
            "Test Collection",
            "Test Description",
            "#FF5733",
            "📁"
        ));

        var collectionId = result.Value;

        // Act
        var response = await Client.GetAsync($"/api/collections/{collectionId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify collection exists in database using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var collection = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id.Value == collectionId);
        collection.Should().NotBeNull();
        collection!.Name.Should().Be("Test Collection");
    }

    [Fact]
    public async Task UpdateCollection_WithValidData_ShouldUpdateCollection()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IMediator>();
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResult = await mediator.Send(new CreateCollectionCommand(
            "Original Name",
            "Original Description",
            "#000000",
            "📁"
        ));

        var collectionId = createResult.Value;

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
            .FirstOrDefaultAsync(c => c.Id.Value == collectionId);
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
        var mediator = Services.GetRequiredService<IMediator>();
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var result = await mediator.Send(new CreateCollectionCommand(
            "To Delete Collection",
            "Will be deleted",
            "#FF0000",
            "🗑️"
        ));

        var collectionId = result.Value;

        // Verify collection exists before deletion
        var collectionBeforeDelete = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id.Value == collectionId);
        collectionBeforeDelete.Should().NotBeNull();

        // Act - Delete via HTTP
        var response = await Client.DeleteAsync($"/api/collections/{collectionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify collection no longer exists
        var collectionAfterDelete = await readContext.Collections
            .FirstOrDefaultAsync(c => c.Id.Value == collectionId);
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
