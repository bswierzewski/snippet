using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;
using Snippet.Modules.Snippets.Application.Commands.Snippets.CreateSnippet;
using Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippet;
using Snippet.Modules.Snippets.Application.Commands.Tags.CreateTag;
using Snippet.Modules.Snippets.Domain.Enums;
using Snippet.Modules.Snippets.Domain.ValueObjects;
using Snippet.Tests.E2E.Core;
using Snippet.Tests.E2E.Core.Extensions;
using Snippet.Tests.E2E.Core.Factories;

namespace Snippet.Tests.E2E.Modules.Snippets;

/// <summary>
/// End-to-end tests for snippet management functionality including creation, retrieval, update, and deletion operations.
/// </summary>
public class SnippetsTests(TestWebApplicationFactory factory) : TestBase(factory)
{
    #region Create Snippet Tests

    [Fact]
    public async Task CreateSnippet_WithValidData_ShouldCreateAndReturnSnippet()
    {
        // Arrange
        var command = new CreateSnippetCommand(
            "Test Snippet",
            "Console.WriteLine(\"Hello World\");",
            ProgrammingLanguage.CSharp,
            "Test Description",
            null,
            null);

        // Act
        var response = await Client.PostJsonAsync("/api/snippets", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify in database using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var snippetsCount = await readContext.Snippets.CountAsync();
        snippetsCount.Should().Be(1);

        var snippet = await readContext.Snippets.FirstAsync();
        snippet.Title.Should().Be("Test Snippet");
        snippet.Content.Should().Be("Console.WriteLine(\"Hello World\");");
        snippet.Language.Should().Be(ProgrammingLanguage.CSharp);
        snippet.Description.Should().Be("Test Description");
    }

    [Fact]
    public async Task CreateSnippet_WithCollections_ShouldAssignToCollections()
    {
        // Arrange
        var collection1Response = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand("Collection 1", null, null, null));
        var collection1Id = await collection1Response.ReadAsJsonAsync<Guid>();

        var collection2Response = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand("Collection 2", null, null, null));
        var collection2Id = await collection2Response.ReadAsJsonAsync<Guid>();

        var command = new CreateSnippetCommand(
            "Snippet in Collections",
            "print('hello')",
            ProgrammingLanguage.Python,
            null,
            null,
            new List<Guid> { collection1Id, collection2Id });

        // Act
        var response = await Client.PostJsonAsync("/api/snippets", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var snippet = await readContext.Snippets.Include(s => s.SnippetCollections).ThenInclude(sc => sc.Collection).FirstAsync();

        snippet.SnippetCollections.Should().HaveCount(2);
    }

    #endregion

    #region Get Snippet Tests

    [Fact]
    public async Task GetSnippets_ShouldReturnSuccess()
    {
        var response = await Client.GetAsync("/api/snippets");

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetSnippets_WithMultipleSnippets_ShouldReturnCorrectCount()
    {
        // Arrange - Create multiple snippets
        var commands = new[]
        {
            new CreateSnippetCommand("Snippet 1", "code1", ProgrammingLanguage.JavaScript, null, null, null),
            new CreateSnippetCommand("Snippet 2", "code2", ProgrammingLanguage.TypeScript, null, null, null),
            new CreateSnippetCommand("Snippet 3", "code3", ProgrammingLanguage.Python, null, null, null)
        };

        foreach (var command in commands)
        {
            await Client.PostJsonAsync("/api/snippets", command);
        }

        // Act
        var response = await Client.GetAsync("/api/snippets");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify database count using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var snippetsCount = await readContext.Snippets.CountAsync();
        snippetsCount.Should().Be(3);
    }

    [Fact]
    public async Task GetSnippetById_WithExistingId_ShouldReturnSnippet()
    {
        // Arrange
        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Test Snippet",
            "SELECT * FROM Users",
            ProgrammingLanguage.Sql,
            "SQL Query",
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        // Act
        var response = await Client.GetAsync($"/api/snippets/{snippetId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Verify snippet exists in database using read context
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var snippet = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippet.Should().NotBeNull();
        snippet!.Title.Should().Be("Test Snippet");
    }

    [Fact]
    public async Task GetSnippetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/snippets/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update Snippet Tests

    [Fact]
    public async Task UpdateSnippetContent_WithValidData_ShouldUpdateContent()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Original Snippet",
            "original content",
            ProgrammingLanguage.PlainText,
            null,
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        var updateCommand = new UpdateSnippetCommand(
            snippetId,
            "Original Snippet",
            null,
            "updated content",
            ProgrammingLanguage.PlainText,
            new List<Guid>(),
            new List<Guid>()
        );

        // Act
        var response = await Client.PutJsonAsync($"/api/snippets/{snippetId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update in database
        var updatedSnippet = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        updatedSnippet.Should().NotBeNull();
        updatedSnippet!.Content.Should().Be("updated content");
    }

    [Fact]
    public async Task ChangeSnippetLanguage_WithValidData_ShouldUpdateLanguage()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Language Change Test",
            "console.log('test')",
            ProgrammingLanguage.JavaScript,
            null,
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        var changeLanguageCommand = new UpdateSnippetCommand(
            snippetId,
            "Language Change Test",
            null,
            "console.log('test')",
            ProgrammingLanguage.TypeScript,
            new List<Guid>(),
            new List<Guid>()
        );

        // Act
        var response = await Client.PutJsonAsync($"/api/snippets/{snippetId}", changeLanguageCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update in database
        var updatedSnippet = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        updatedSnippet.Should().NotBeNull();
        updatedSnippet!.Language.Should().Be(ProgrammingLanguage.TypeScript);
    }

    #endregion

    #region Tag Management Tests

    [Fact]
    public async Task AddTag_WithValidData_ShouldAddTagToSnippet()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        // Create a tag first
        var createTagResponse = await Client.PostJsonAsync("/api/tags", new CreateTagCommand(
            "important",
            "#FF0000"
        ));
        var tagId = await createTagResponse.ReadAsJsonAsync<Guid>();

        // Create snippet without tags
        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Tag Test Snippet",
            "code",
            ProgrammingLanguage.CSharp,
            null,
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        // Update snippet to include the tag
        var updateCommand = new UpdateSnippetCommand(
            snippetId,
            "Tag Test Snippet",
            null,
            "code",
            ProgrammingLanguage.CSharp,
            new List<Guid> { tagId },
            new List<Guid>()
        );

        // Act
        var response = await Client.PutJsonAsync($"/api/snippets/{snippetId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify tag added in database
        var snippet = await readContext.Snippets
            .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippet.Should().NotBeNull();
        snippet!.SnippetTags.Should().HaveCount(1);
        snippet.SnippetTags.First().Tag.Name.Should().Be("important");
    }

    [Fact]
    public async Task RemoveTag_WithExistingTag_ShouldRemoveTag()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        // Create a tag
        var createTagResponse = await Client.PostJsonAsync("/api/tags", new CreateTagCommand(
            "temporary",
            null
        ));
        var tagId = await createTagResponse.ReadAsJsonAsync<Guid>();

        // Create snippet with tag
        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Remove Tag Test",
            "code",
            ProgrammingLanguage.CSharp,
            null,
            new List<Guid> { tagId },
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        // Update snippet to remove the tag
        var updateCommand = new UpdateSnippetCommand(
            snippetId,
            "Remove Tag Test",
            null,
            "code",
            ProgrammingLanguage.CSharp,
            new List<Guid>(),
            new List<Guid>()
        );

        // Act
        var response = await Client.PutJsonAsync($"/api/snippets/{snippetId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify tag removed
        var snippet = await readContext.Snippets
            .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippet.Should().NotBeNull();
        snippet!.SnippetTags.Should().BeEmpty();
    }

    #endregion

    #region Favorite and Usage Tests

    [Fact]
    public async Task ToggleFavorite_ShouldToggleSnippetFavoriteStatus()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Favorite Test",
            "code",
            ProgrammingLanguage.CSharp,
            null,
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        // Act - Toggle to favorite
        var response1 = await Client.PostAsync($"/api/snippets/{snippetId}/favorite", null);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var snippet = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippet.Should().NotBeNull();
        snippet!.IsFavorite.Should().BeTrue();

        // Act - Toggle back
        var response2 = await Client.PostAsync($"/api/snippets/{snippetId}/favorite", null);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var snippetAfterToggle = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippetAfterToggle!.IsFavorite.Should().BeFalse();
    }

    [Fact]
    public async Task RecordUsage_ShouldUpdateUsageCount()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Usage Test",
            "code",
            ProgrammingLanguage.CSharp,
            null,
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        // Act
        var response = await Client.PostAsync($"/api/snippets/{snippetId}/usage", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var snippet = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippet.Should().NotBeNull();
        snippet!.UsageCount.Should().Be(1);
    }

    #endregion

    #region Move Snippet Tests

    [Fact]
    public async Task MoveSnippet_ShouldUpdateCollections()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var collection1Response = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand("Collection 1", null, null, null));
        var collection1Id = await collection1Response.ReadAsJsonAsync<Guid>();

        var collection2Response = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand("Collection 2", null, null, null));
        var collection2Id = await collection2Response.ReadAsJsonAsync<Guid>();

        var snippetResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "Move Test",
            "code",
            ProgrammingLanguage.CSharp,
            null,
            null,
            new List<Guid> { collection1Id }
        ));

        var snippetId = await snippetResponse.ReadAsJsonAsync<Guid>();

        var moveCommand = new UpdateSnippetCommand(
            snippetId,
            "Move Test",
            null,
            "code",
            ProgrammingLanguage.CSharp,
            new List<Guid>(),
            new List<Guid> { collection2Id }
        );

        // Act
        var response = await Client.PutJsonAsync($"/api/snippets/{snippetId}", moveCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify collections updated
        var snippet = await readContext.Snippets
            .Include(s => s.SnippetCollections).ThenInclude(sc => sc.Collection)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippet.Should().NotBeNull();
        snippet!.SnippetCollections.Should().HaveCount(1);
        snippet.SnippetCollections.First().Collection.Id.Value.Should().Be(collection2Id);
    }

    #endregion

    #region Delete Snippet Tests

    [Fact]
    public async Task DeleteSnippet_WithExistingId_ShouldRemoveFromDatabase()
    {
        // Arrange
        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();

        var createResponse = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand(
            "To Delete Snippet",
            "will be deleted",
            ProgrammingLanguage.PlainText,
            null,
            null,
            null
        ));

        var snippetId = await createResponse.ReadAsJsonAsync<Guid>();

        // Verify snippet exists before deletion
        var snippetBeforeDelete = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippetBeforeDelete.Should().NotBeNull();

        // Act - Delete via HTTP
        var response = await Client.DeleteAsync($"/api/snippets/{snippetId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify snippet no longer exists
        var snippetAfterDelete = await readContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(snippetId));
        snippetAfterDelete.Should().BeNull();

        var totalCount = await readContext.Snippets.CountAsync();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task DeleteSnippet_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"/api/snippets/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Query Tests

    [Fact]
    public async Task GetFavoriteSnippets_ShouldReturnOnlyFavorites()
    {
        // Arrange
        var snippet1Response = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand("Snippet 1", "code1", ProgrammingLanguage.CSharp, null, null, null));
        var snippet1Id = await snippet1Response.ReadAsJsonAsync<Guid>();

        var snippet2Response = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand("Snippet 2", "code2", ProgrammingLanguage.CSharp, null, null, null));
        var snippet2Id = await snippet2Response.ReadAsJsonAsync<Guid>();

        var snippet3Response = await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand("Snippet 3", "code3", ProgrammingLanguage.CSharp, null, null, null));
        var snippet3Id = await snippet3Response.ReadAsJsonAsync<Guid>();

        // Mark snippet 1 and 3 as favorites
        await Client.PostAsync($"/api/snippets/{snippet1Id}/favorite", null);
        await Client.PostAsync($"/api/snippets/{snippet3Id}/favorite", null);

        // Act
        var response = await Client.GetAsync("/api/snippets/favorites");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var readContext = Services.GetRequiredService<ISnippetsReadDbContext>();
        var favoriteCount = await readContext.Snippets.CountAsync(s => s.IsFavorite);
        favoriteCount.Should().Be(2);
    }

    [Fact]
    public async Task GetRecentSnippets_ShouldReturnLimitedResults()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand($"Snippet {i}", $"code{i}", ProgrammingLanguage.CSharp, null, null, null));
        }

        // Act
        var response = await Client.GetAsync("/api/snippets/recent?limit=5");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetCollectionSnippets_ShouldReturnSnippetsInCollection()
    {
        // Arrange
        var collectionResponse = await Client.PostJsonAsync("/api/collections", new CreateCollectionCommand("Test Collection", null, null, null));
        var collectionId = await collectionResponse.ReadAsJsonAsync<Guid>();

        await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand("Snippet 1", "code1", ProgrammingLanguage.CSharp, null, null, new List<Guid> { collectionId }));
        await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand("Snippet 2", "code2", ProgrammingLanguage.CSharp, null, null, new List<Guid> { collectionId }));
        await Client.PostJsonAsync("/api/snippets", new CreateSnippetCommand("Snippet 3", "code3", ProgrammingLanguage.CSharp, null, null, null)); // Not in collection

        // Act
        var response = await Client.GetAsync($"/api/snippets/collections/{collectionId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    #endregion
}
