using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGo.Migrations
{
    /// <inheritdoc />
    public partial class mobileNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "mobile_number",
                table: "bookings",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mobile_number",
                table: "bookings");
        }
    }
}
