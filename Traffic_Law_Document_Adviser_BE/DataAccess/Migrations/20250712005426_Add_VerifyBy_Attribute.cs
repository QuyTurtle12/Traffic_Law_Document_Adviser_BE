using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_VerifyBy_Attribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VerifyBy",
                table: "LawDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LawDocuments_VerifyBy",
                table: "LawDocuments",
                column: "VerifyBy");

            migrationBuilder.AddForeignKey(
                name: "FK_LawDocuments_Users_VerifyBy",
                table: "LawDocuments",
                column: "VerifyBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LawDocuments_Users_VerifyBy",
                table: "LawDocuments");

            migrationBuilder.DropIndex(
                name: "IX_LawDocuments_VerifyBy",
                table: "LawDocuments");

            migrationBuilder.DropColumn(
                name: "VerifyBy",
                table: "LawDocuments");
        }
    }
}
