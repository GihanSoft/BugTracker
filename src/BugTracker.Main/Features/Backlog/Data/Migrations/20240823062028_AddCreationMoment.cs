using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Main.Features.Backlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCreationMoment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "creation_moment",
                schema: "backlog",
                table: "project",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("""
                UPDATE backlog.project x
                SET creation_moment =
                    CURRENT_TIMESTAMP +
                    interval '-1 day' +
                    make_interval(mins => x.id)
                """);

            migrationBuilder.AddColumn<DateTime>(
                name: "creation_moment",
                schema: "backlog",
                table: "product_backlog_item",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("""
                UPDATE backlog.product_backlog_item x
                SET creation_moment =
                    CURRENT_TIMESTAMP +
                    interval '-1 day' +
                    make_interval(mins => x.id::int)
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creation_moment",
                schema: "backlog",
                table: "project");

            migrationBuilder.DropColumn(
                name: "creation_moment",
                schema: "backlog",
                table: "product_backlog_item");
        }
    }
}
