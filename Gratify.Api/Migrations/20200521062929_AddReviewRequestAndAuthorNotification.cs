using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class AddReviewRequestAndAuthorNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorNotificationChannel",
                table: "Reviews",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorNotificationTimestamp",
                table: "Reviews",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewRequestChannel",
                table: "Reviews",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewRequestTimestamp",
                table: "Reviews",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorNotificationChannel",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AuthorNotificationTimestamp",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewRequestChannel",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewRequestTimestamp",
                table: "Reviews");
        }
    }
}
