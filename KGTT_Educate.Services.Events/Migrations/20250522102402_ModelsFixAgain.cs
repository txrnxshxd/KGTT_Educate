using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGTT_Educate.Services.Events.Migrations
{
    /// <inheritdoc />
    public partial class ModelsFixAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventGroup_Group_GroupId",
                schema: "Events",
                table: "EventGroup");

            migrationBuilder.DropTable(
                name: "Group",
                schema: "Events");

            migrationBuilder.DropIndex(
                name: "IX_EventGroup_GroupId",
                schema: "Events",
                table: "EventGroup");

            migrationBuilder.DropColumn(
                name: "GroupId",
                schema: "Events",
                table: "EventGroup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                schema: "Events",
                table: "EventGroup",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Group",
                schema: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventGroup_GroupId",
                schema: "Events",
                table: "EventGroup",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventGroup_Group_GroupId",
                schema: "Events",
                table: "EventGroup",
                column: "GroupId",
                principalSchema: "Events",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
