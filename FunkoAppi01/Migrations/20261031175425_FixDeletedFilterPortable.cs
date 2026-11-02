using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunkoAppi01.Migrations
{
    /// <inheritdoc />
    public partial class FixDeletedFilterPortable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_funkos_nombre",
                table: "funkos");

            migrationBuilder.CreateIndex(
                name: "IX_funkos_nombre",
                table: "funkos",
                column: "nombre",
                unique: true,
                filter: "\"is_deleted\" = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_funkos_nombre",
                table: "funkos");

            migrationBuilder.CreateIndex(
                name: "IX_funkos_nombre",
                table: "funkos",
                column: "nombre",
                unique: true,
                filter: "\"is_deleted\" = 0");
        }
    }
}
