using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class RemoveDrafts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grats_Drafts_DraftId",
                table: "Grats");

            migrationBuilder.DropTable(
                name: "Drafts");

            migrationBuilder.DropIndex(
                name: "IX_Grats_DraftId",
                table: "Grats");

            migrationBuilder.DropColumn(
                name: "DraftId",
                table: "Grats");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Grats",
                maxLength: 100,
                nullable: false,
                defaultValue: string.Empty);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Grats");

            migrationBuilder.AddColumn<int>(
                name: "DraftId",
                table: "Grats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grats_DraftId",
                table: "Grats",
                column: "DraftId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Grats_Drafts_DraftId",
                table: "Grats",
                column: "DraftId",
                principalTable: "Drafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
