using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class ApprovalReceiverNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotified",
                table: "Approvals");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverNotificationChannel",
                table: "Approvals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverNotificationTimestamp",
                table: "Approvals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverNotificationChannel",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ReceiverNotificationTimestamp",
                table: "Approvals");

            migrationBuilder.AddColumn<bool>(
                name: "IsNotified",
                table: "Approvals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
