using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class ReceiverNameAndAccountNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverAccountNumber",
                table: "Receivals",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Receivals",
                nullable: false,
                defaultValue: string.Empty);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverAccountNumber",
                table: "Receivals");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Receivals");
        }
    }
}
