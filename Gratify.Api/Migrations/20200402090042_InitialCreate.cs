using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Gratify.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Author = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<string>(maxLength: 100, nullable: true),
                    DefaultReviewer = table.Column<string>(maxLength: 100, nullable: false),
                    HasReports = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Recipient = table.Column<string>(maxLength: 100, nullable: false),
                    Challenge = table.Column<string>(maxLength: 300, nullable: false),
                    Action = table.Column<string>(maxLength: 300, nullable: false),
                    Result = table.Column<string>(maxLength: 300, nullable: false),
                    DraftId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grats_Drafts_DraftId",
                        column: x => x.DraftId,
                        principalTable: "Drafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    RequestedAt = table.Column<DateTime>(nullable: false),
                    Reviewer = table.Column<string>(maxLength: 100, nullable: false),
                    ForwardedFrom = table.Column<int>(nullable: true),
                    GratsId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Grats_GratsId",
                        column: x => x.GratsId,
                        principalTable: "Grats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    ApprovedAt = table.Column<DateTime>(nullable: false),
                    ReviewId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvals_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Denials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    DeniedAt = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(maxLength: 500, nullable: false),
                    ReviewId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Denials_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receivals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(nullable: false),
                    TeamId = table.Column<string>(maxLength: 100, nullable: false),
                    ReceivedAt = table.Column<DateTime>(nullable: false),
                    ApprovalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receivals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receivals_Approvals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalTable: "Approvals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ReviewId",
                table: "Approvals",
                column: "ReviewId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Denials_ReviewId",
                table: "Denials",
                column: "ReviewId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grats_DraftId",
                table: "Grats",
                column: "DraftId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receivals_ApprovalId",
                table: "Receivals",
                column: "ApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GratsId",
                table: "Reviews",
                column: "GratsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Denials");

            migrationBuilder.DropTable(
                name: "Receivals");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Grats");

            migrationBuilder.DropTable(
                name: "Drafts");
        }
    }
}
