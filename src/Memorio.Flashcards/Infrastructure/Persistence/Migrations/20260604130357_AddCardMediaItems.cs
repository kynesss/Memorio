using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Memorio.Flashcards.Infrastructure.Persistence.Migrations
{
    public partial class AddCardMediaItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardMediaItems",
                schema: "flashcards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ObjectKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardMediaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardMediaItems_Cards_CardId",
                        column: x => x.CardId,
                        principalSchema: "flashcards",
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardMediaItems_CardId",
                schema: "flashcards",
                table: "CardMediaItems",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardMediaItems_ObjectKey",
                schema: "flashcards",
                table: "CardMediaItems",
                column: "ObjectKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardMediaItems",
                schema: "flashcards");
        }
    }
}
