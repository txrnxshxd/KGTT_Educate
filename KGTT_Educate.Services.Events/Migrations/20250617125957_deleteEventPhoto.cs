using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGTT_Educate.Services.Events.Migrations
{
    /// <inheritdoc />
    public partial class deleteEventPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaFullPath",
                schema: "Events",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MediaLocalPath",
                schema: "Events",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaFullPath",
                schema: "Events",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaLocalPath",
                schema: "Events",
                table: "Events",
                type: "text",
                nullable: true);
        }
    }
}
