using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGTT_Educate.Services.Account.Migrations
{
    /// <inheritdoc />
    public partial class AddedAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarFullPath",
                schema: "Users",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AvatarLocalPath",
                schema: "Users",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarFullPath",
                schema: "Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AvatarLocalPath",
                schema: "Users",
                table: "Users");
        }
    }
}
