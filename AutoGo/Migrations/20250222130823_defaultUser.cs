using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGo.Migrations
{
    /// <inheritdoc />
    public partial class defaultUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert the default users
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "name", "mobile_number", "pin", "is_verified", "user_type", "OrganizationIds" },
                values: new object[] { 1, "dev_user", "9874563211", "1111", true, 2, new[] { 1 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
