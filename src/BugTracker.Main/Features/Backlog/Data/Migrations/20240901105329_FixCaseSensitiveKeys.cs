using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.Internal;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCaseSensitiveKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tag_key_project_id",
                schema: "backlog",
                table: "tag");

            migrationBuilder.DropIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:backlog.case_insensitive", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX ix_tag_key_project_id ON backlog.tag (key COLLATE backlog.case_insensitive, project_id);
                """);

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX ix_project_owner_key_key ON backlog.project (owner_key COLLATE backlog.case_insensitive, key COLLATE backlog.case_insensitive);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tag_key_project_id",
                schema: "backlog",
                table: "tag");

            migrationBuilder.DropIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:CollationDefinition:backlog.case_insensitive", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.CreateIndex(
                name: "ix_tag_key_project_id",
                schema: "backlog",
                table: "tag",
                columns: new[] { "key", "project_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project",
                columns: new[] { "owner_key", "key" },
                unique: true);
        }
    }
}
