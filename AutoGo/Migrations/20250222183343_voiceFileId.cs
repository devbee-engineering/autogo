using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGo.Migrations
{
    /// <inheritdoc />
    public partial class voiceFileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "voice_file_id",
                table: "bookings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "voice_file_id",
                table: "bookings");
        }
    }
}
