using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectKeyUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project");

            migrationBuilder.CreateIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project",
                columns: new[] { "owner_key", "key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project");

            migrationBuilder.CreateIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project",
                columns: new[] { "owner_key", "key" });
        }
    }
}
