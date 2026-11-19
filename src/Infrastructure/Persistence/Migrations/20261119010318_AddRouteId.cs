using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coordinates_Client_CreatedAt",
                table: "Coordinates");

            migrationBuilder.AddColumn<string>(
                name: "RouteId",
                table: "Coordinates",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_Client_RouteId_CreatedAt",
                table: "Coordinates",
                columns: new[] { "Client", "RouteId", "CreatedAt" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coordinates_Client_RouteId_CreatedAt",
                table: "Coordinates");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Coordinates");

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_Client_CreatedAt",
                table: "Coordinates",
                columns: new[] { "Client", "CreatedAt" },
                unique: true);
        }
    }
}
