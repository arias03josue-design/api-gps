using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreatedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coordinates_CreatedAt",
                table: "Coordinates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_CreatedAt",
                table: "Coordinates",
                column: "CreatedAt");
        }
    }
}
