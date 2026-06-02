using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Learning.Infrastructure.Persistence.Migrations
{
    public partial class InitialLearningSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "learning");

            migrationBuilder.CreateTable(
                name: "CardProgresses",
                schema: "learning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    DueAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Step = table.Column<int>(type: "integer", nullable: true),
                    Stability = table.Column<double>(type: "double precision", nullable: true),
                    Difficulty = table.Column<double>(type: "double precision", nullable: true),
                    LastReviewAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardProgresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudySessions",
                schema: "learning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeckId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardReviews",
                schema: "learning",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextReviewAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewDurationMs = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardReviews_StudySessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "learning",
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_CardId",
                schema: "learning",
                table: "CardProgresses",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_UserId",
                schema: "learning",
                table: "CardProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_UserId_CardId",
                schema: "learning",
                table: "CardProgresses",
                columns: new[] { "UserId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_UserId_DueAt",
                schema: "learning",
                table: "CardProgresses",
                columns: new[] { "UserId", "DueAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CardReviews_CardId",
                schema: "learning",
                table: "CardReviews",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardReviews_ReviewedAt",
                schema: "learning",
                table: "CardReviews",
                column: "ReviewedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CardReviews_SessionId",
                schema: "learning",
                table: "CardReviews",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_DeckId",
                schema: "learning",
                table: "StudySessions",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_UserId",
                schema: "learning",
                table: "StudySessions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardProgresses",
                schema: "learning");

            migrationBuilder.DropTable(
                name: "CardReviews",
                schema: "learning");

            migrationBuilder.DropTable(
                name: "StudySessions",
                schema: "learning");
        }
    }
}
