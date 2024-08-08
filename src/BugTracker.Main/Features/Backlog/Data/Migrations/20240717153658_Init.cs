using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "backlog");

            migrationBuilder.CreateTable(
                name: "project",
                schema: "backlog",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    owner_key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_backlog_item",
                schema: "backlog",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    project_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_backlog_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_backlog_item_project_project_id",
                        column: x => x.project_id,
                        principalSchema: "backlog",
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_product_backlog_item_project_id",
                schema: "backlog",
                table: "product_backlog_item",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_owner_key_key",
                schema: "backlog",
                table: "project",
                columns: new[] { "owner_key", "key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_backlog_item",
                schema: "backlog");

            migrationBuilder.DropTable(
                name: "project",
                schema: "backlog");
        }
    }
}
