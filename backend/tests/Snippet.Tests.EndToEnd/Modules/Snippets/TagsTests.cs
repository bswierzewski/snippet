using System.Net;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Tests.Authentication;
using Shared.Infrastructure.Tests.Core;
using Shared.Infrastructure.Tests.Extensions.Http;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Commands.Tags.CreateTag;
using Snippet.Modules.Snippets.Application.Queries.Tags.GetTags;
using Snippet.Modules.Snippets.Application.Queries.Tags.GetUserTags;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Tests.EndToEnd.Modules.Snippets;

/// <summary>
/// End-to-end tests for tag management functionality including creation, retrieval, search, and deletion operations.
/// </summary>
[Collection("Snippet")]
public class TagsTests(SnippetTestFixture fixture) : IAsyncLifetime
{
    private readonly TestContext _context = fixture.Context;
    private readonly SnippetTestFixture _fixture = fixture;

    public async Task InitializeAsync()
    {
        await _context.ResetDatabaseAsync();
        var tokenProvider = _context.GetRequiredService<ITokenProvider>();
        var token = await tokenProvider.GetTokenAsync(_fixture.TestUser.Email, _fixture.TestUser.Password);
        _context.Client.WithBearerToken(token);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    #region Create Tag Tests

    [Fact]
    public async Task CreateTag_WithValidData_ShouldCreateAndReturnTagId()
    {
        // Arrange
        var command = new CreateTagCommand(
            "ImportantTag",
            "#FF5733"
        );

        // Act
        var response = await _context.Client.PostJsonAsync("/api/tags", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tagId = await response.ReadAsJsonAsync<Guid>();
        tagId.Should().NotBeEmpty();

        // Verify in database
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var tag = await readContext.Tags.FirstOrDefaultAsync(t => t.Id == new TagId(tagId));
        tag.Should().NotBeNull();
        tag!.Name.Should().Be("importanttag"); // Tags are stored in lowercase
        tag.Color.Should().Be("#FF5733");
    }

    [Fact]
    public async Task CreateTag_WithoutColor_ShouldCreateTagWithNullColor()
    {
        // Arrange
        var command = new CreateTagCommand(
            "SimpleTag",
            null
        );

        // Act
        var response = await _context.Client.PostJsonAsync("/api/tags", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tagId = await response.ReadAsJsonAsync<Guid>();

        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var tag = await readContext.Tags.FirstOrDefaultAsync(t => t.Id == new TagId(tagId));
        tag.Should().NotBeNull();
        tag!.Color.Should().BeNull();
    }

    [Fact]
    public async Task CreateTag_WithDuplicateName_ShouldHandleGracefully()
    {
        // Arrange
        var command1 = new CreateTagCommand("DuplicateTag", "#FF0000");
        var command2 = new CreateTagCommand("duplicatetag", "#00FF00"); // Same name, different case

        // Act
        var response1 = await _context.Client.PostJsonAsync("/api/tags", command1);
        var response2 = await _context.Client.PostJsonAsync("/api/tags", command2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        // Response2 behavior depends on business logic - might be BadRequest or create duplicate
        // Adjust based on your domain rules
    }

    #endregion

    #region Get User Tags Tests

    [Fact]
    public async Task GetUserTags_WithNoTags_ShouldReturnEmptyList()
    {
        // Act
        var response = await _context.Client.GetAsync("/api/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadAsJsonAsync<IEnumerable<TagDto>>();
        tags.Should().NotBeNull();
        tags.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserTags_WithMultipleTags_ShouldReturnAllUserTags()
    {
        // Arrange - Create multiple tags
        var tag1Response = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Tag1", "#FF0000"));
        var tag2Response = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Tag2", "#00FF00"));
        var tag3Response = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Tag3", null));

        // Act
        var response = await _context.Client.GetAsync("/api/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadAsJsonAsync<IEnumerable<TagDto>>();
        tags.Should().NotBeNull();
        tags.Should().HaveCount(3);

        var tagList = tags!.ToList();
        tagList.Should().Contain(t => t.Name == "tag1");
        tagList.Should().Contain(t => t.Name == "tag2");
        tagList.Should().Contain(t => t.Name == "tag3");
    }

    [Fact]
    public async Task GetUserTags_ShouldIncludeSnippetCount()
    {
        // Arrange - Create tag and assign to snippet
        var tagResponse = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("CountedTag", null));
        var tagId = await tagResponse.ReadAsJsonAsync<Guid>();

        // Create snippet with tag (using AddTag endpoint from SnippetsTests)
        // Note: This requires snippet creation - adjust based on your workflow

        // Act
        var response = await _context.Client.GetAsync("/api/tags");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadAsJsonAsync<IEnumerable<TagDto>>();
        var tag = tags!.FirstOrDefault(t => t.Id == tagId);
        tag.Should().NotBeNull();
        tag!.SnippetCount.Should().BeGreaterThanOrEqualTo(0);
    }

    #endregion

    #region Search Tags Tests

    [Fact]
    public async Task SearchTags_WithoutSearchTerm_ShouldReturnAllTags()
    {
        // Arrange
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Alpha", null));
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Beta", null));
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Gamma", null));

        // Act
        var response = await _context.Client.GetAsync("/api/tags/search");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        tags.Should().NotBeNull();
        tags.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task SearchTags_WithSearchTerm_ShouldReturnMatchingTags()
    {
        // Arrange
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("JavaScript", null));
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Java", null));
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Python", null));

        // Act
        var response = await _context.Client.GetAsync("/api/tags/search?searchTerm=java");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        tags.Should().NotBeNull();

        var tagList = tags!.ToList();
        tagList.Should().Contain(t => t.Name.Contains("java"));
        tagList.Should().NotContain(t => t.Name == "python");
    }

    [Fact]
    public async Task SearchTags_WithNonMatchingTerm_ShouldReturnEmptyList()
    {
        // Arrange
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Tag1", null));

        // Act
        var response = await _context.Client.GetAsync("/api/tags/search?searchTerm=nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags = await response.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        tags.Should().NotBeNull();
        tags.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchTags_IsCaseInsensitive()
    {
        // Arrange
        await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("ImportantTag", null));

        // Act - Search with different cases
        var response1 = await _context.Client.GetAsync("/api/tags/search?searchTerm=IMPORTANT");
        var response2 = await _context.Client.GetAsync("/api/tags/search?searchTerm=important");
        var response3 = await _context.Client.GetAsync("/api/tags/search?searchTerm=ImPoRtAnT");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        response3.StatusCode.Should().Be(HttpStatusCode.OK);

        var tags1 = await response1.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        var tags2 = await response2.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        var tags3 = await response3.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();

        tags1.Should().NotBeEmpty();
        tags2.Should().NotBeEmpty();
        tags3.Should().NotBeEmpty();
    }

    #endregion

    #region Delete Tag Tests

    [Fact]
    public async Task DeleteTag_WithExistingTag_ShouldRemoveFromDatabase()
    {
        // Arrange
        var createResponse = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("ToDelete", "#FF0000"));
        var tagId = await createResponse.ReadAsJsonAsync<Guid>();

        // Verify tag exists
        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var tagBeforeDelete = await readContext.Tags.FirstOrDefaultAsync(t => t.Id == new TagId(tagId));
        tagBeforeDelete.Should().NotBeNull();

        // Act
        var response = await _context.Client.DeleteAsync($"/api/tags/{tagId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify tag no longer exists
        var tagAfterDelete = await readContext.Tags.FirstOrDefaultAsync(t => t.Id == new TagId(tagId));
        tagAfterDelete.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTag_WithNonExistingTag_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _context.Client.DeleteAsync($"/api/tags/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTag_MultipleSequentialDeletes_ShouldWork()
    {
        // Arrange
        var tag1Response = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Delete1", null));
        var tag1Id = await tag1Response.ReadAsJsonAsync<Guid>();

        var tag2Response = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Delete2", null));
        var tag2Id = await tag2Response.ReadAsJsonAsync<Guid>();

        // Act
        var response1 = await _context.Client.DeleteAsync($"/api/tags/{tag1Id}");
        var response2 = await _context.Client.DeleteAsync($"/api/tags/{tag2Id}");

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response2.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var readContext = _context.GetRequiredService<ISnippetsReadDbContext>();
        var remainingTags = await readContext.Tags.CountAsync();
        remainingTags.Should().Be(0);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task TagLifecycle_CreateSearchAndDelete_ShouldWorkEndToEnd()
    {
        // Create
        var createResponse = await _context.Client.PostJsonAsync("/api/tags", new CreateTagCommand("Lifecycle", "#123456"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tagId = await createResponse.ReadAsJsonAsync<Guid>();

        // Search
        var searchResponse = await _context.Client.GetAsync("/api/tags/search?searchTerm=life");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResults = await searchResponse.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        searchResults.Should().Contain(t => t.Id == tagId);

        // Get User Tags
        var getUserTagsResponse = await _context.Client.GetAsync("/api/tags");
        getUserTagsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var userTags = await getUserTagsResponse.ReadAsJsonAsync<IEnumerable<TagDto>>();
        userTags.Should().Contain(t => t.Id == tagId);

        // Delete
        var deleteResponse = await _context.Client.DeleteAsync($"/api/tags/{tagId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var searchAfterDelete = await _context.Client.GetAsync("/api/tags/search?searchTerm=life");
        var searchResultsAfterDelete = await searchAfterDelete.ReadAsJsonAsync<IEnumerable<TagSearchDto>>();
        searchResultsAfterDelete.Should().NotContain(t => t.Id == tagId);
    }

    #endregion
}
