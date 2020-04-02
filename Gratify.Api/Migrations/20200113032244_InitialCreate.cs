using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sender = table.Column<string>(maxLength: 100, nullable: false),
                    Content = table.Column<string>(maxLength: 500, nullable: false),
                    Approver = table.Column<string>(maxLength: 100, nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    Receiver = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grats", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grats");
        }
    }
}
