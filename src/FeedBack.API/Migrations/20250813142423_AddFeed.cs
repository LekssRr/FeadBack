using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeadBack.Migrations
{
    /// <inheritdoc />
    public partial class AddFeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "feed",
                table: "FeadBacks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "feed",
                table: "FeadBacks");
        }
    }
}
