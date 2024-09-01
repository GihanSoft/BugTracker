using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tag",
                schema: "backlog",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    creation_moment = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    project_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag", x => x.id);
                    table.ForeignKey(
                        name: "fk_tag_project_project_id",
                        column: x => x.project_id,
                        principalSchema: "backlog",
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pbi_tag",
                schema: "backlog",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    creation_moment = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pbi_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pbi_tag", x => x.id);
                    table.ForeignKey(
                        name: "fk_pbi_tag_product_backlog_item_pbi_id",
                        column: x => x.pbi_id,
                        principalSchema: "backlog",
                        principalTable: "product_backlog_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pbi_tag_tag_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "backlog",
                        principalTable: "tag",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pbi_tag_pbi_id_tag_id",
                schema: "backlog",
                table: "pbi_tag",
                columns: new[] { "pbi_id", "tag_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pbi_tag_tag_id",
                schema: "backlog",
                table: "pbi_tag",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_tag_key_project_id",
                schema: "backlog",
                table: "tag",
                columns: new[] { "key", "project_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tag_project_id",
                schema: "backlog",
                table: "tag",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pbi_tag",
                schema: "backlog");

            migrationBuilder.DropTable(
                name: "tag",
                schema: "backlog");
        }
    }
}
