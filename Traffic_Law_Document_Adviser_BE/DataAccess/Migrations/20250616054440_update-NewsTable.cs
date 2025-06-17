using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateNewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First add a temporary column
            migrationBuilder.AddColumn<int>(
                name: "embeddedNewsId_New",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Drop the original column
            migrationBuilder.DropColumn(
                name: "embeddedNewsId",
                table: "News");

            // Add the new int column
            migrationBuilder.AddColumn<int>(
                name: "embeddedNewsId",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Copy data from temporary column
            migrationBuilder.Sql("UPDATE News SET embeddedNewsId = embeddedNewsId_New");

            // Drop the temporary column
            migrationBuilder.DropColumn(
                name: "embeddedNewsId_New",
                table: "News");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // First add a temporary column
            migrationBuilder.AddColumn<Guid>(
                name: "embeddedNewsId_New",
                table: "News",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            // Drop the original column
            migrationBuilder.DropColumn(
                name: "embeddedNewsId",
                table: "News");

            // Add back the GUID column
            migrationBuilder.AddColumn<Guid>(
                name: "embeddedNewsId",
                table: "News",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            // Copy data (Note: this will generate new GUIDs)
            migrationBuilder.Sql("UPDATE News SET embeddedNewsId = embeddedNewsId_New");

            // Drop the temporary column
            migrationBuilder.DropColumn(
                name: "embeddedNewsId_New",
                table: "News");
        }
    }
}