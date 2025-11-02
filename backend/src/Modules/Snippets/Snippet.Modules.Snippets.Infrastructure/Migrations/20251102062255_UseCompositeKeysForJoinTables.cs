using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snippet.Modules.Snippets.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseCompositeKeysForJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SnippetTags_SnippetId",
                table: "SnippetTags");

            migrationBuilder.DropIndex(
                name: "IX_SnippetCollections_SnippetId",
                table: "SnippetCollections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SnippetTags_SnippetId",
                table: "SnippetTags",
                column: "SnippetId");

            migrationBuilder.CreateIndex(
                name: "IX_SnippetCollections_SnippetId",
                table: "SnippetCollections",
                column: "SnippetId");
        }
    }
}
