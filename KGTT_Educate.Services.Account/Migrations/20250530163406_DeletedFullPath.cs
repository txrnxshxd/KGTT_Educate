using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KGTT_Educate.Services.Account.Migrations
{
    /// <inheritdoc />
    public partial class DeletedFullPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarFullPath",
                schema: "Users",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarFullPath",
                schema: "Users",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
