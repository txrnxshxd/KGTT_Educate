using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGTT_Educate.Services.Events.Migrations
{
    /// <inheritdoc />
    public partial class ModelsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_UserDTO_UserId",
                schema: "Events",
                table: "EventUser");

            migrationBuilder.DropTable(
                name: "UserDTO",
                schema: "Events");

            migrationBuilder.DropIndex(
                name: "IX_EventUser_UserId",
                schema: "Events",
                table: "EventUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDTO",
                schema: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Telegram = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDTO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_UserId",
                schema: "Events",
                table: "EventUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_UserDTO_UserId",
                schema: "Events",
                table: "EventUser",
                column: "UserId",
                principalSchema: "Events",
                principalTable: "UserDTO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
