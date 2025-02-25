using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGo.Migrations
{
    /// <inheritdoc />
    public partial class defaultOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert the default organization
            migrationBuilder.InsertData(
                table: "organizations",
                columns: new[] { "id", "name" },
                values: new object[] { 1, "Default" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
