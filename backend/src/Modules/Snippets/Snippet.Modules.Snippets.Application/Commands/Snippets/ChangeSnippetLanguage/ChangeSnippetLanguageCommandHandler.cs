using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.ChangeSnippetLanguage;

/// <summary>
/// Handles changing snippet language by processing ChangeSnippetLanguageCommand requests.
/// </summary>
public class ChangeSnippetLanguageCommandHandler : IRequestHandler<ChangeSnippetLanguageCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public ChangeSnippetLanguageCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Changes the programming language of an existing snippet.
    /// </summary>
    /// <param name="request">Command containing snippet ID and new language.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(ChangeSnippetLanguageCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.Id} not found");

        snippet.ChangeLanguage(request.Language);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
