using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class DraftsEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sender = table.Column<string>(maxLength: 100, nullable: false),
                    Content = table.Column<string>(maxLength: 500, nullable: true),
                    IsSubmitted = table.Column<bool>(nullable: false),
                    Receiver = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drafts");
        }
    }
}
