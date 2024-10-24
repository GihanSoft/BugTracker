using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pbi_tag_pbi_id_tag_id",
                schema: "backlog",
                table: "pbi_tag");

            migrationBuilder.CreateIndex(
                name: "ix_pbi_tag_pbi_id",
                schema: "backlog",
                table: "pbi_tag",
                column: "pbi_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_pbi_tag_pbi_id",
                schema: "backlog",
                table: "pbi_tag");

            migrationBuilder.CreateIndex(
                name: "ix_pbi_tag_pbi_id_tag_id",
                schema: "backlog",
                table: "pbi_tag",
                columns: new[] { "pbi_id", "tag_id" },
                unique: true);
        }
    }
}
