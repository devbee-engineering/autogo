using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGo.Migrations
{
    /// <inheritdoc />
    public partial class userOnline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_online",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_online",
                table: "users");
        }
    }
}
