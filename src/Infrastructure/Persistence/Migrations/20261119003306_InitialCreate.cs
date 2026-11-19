using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGps.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coordinates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Client = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Coordinate = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_Client_CreatedAt",
                table: "Coordinates",
                columns: new[] { "Client", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coordinates_CreatedAt",
                table: "Coordinates",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinates");
        }
    }
}
