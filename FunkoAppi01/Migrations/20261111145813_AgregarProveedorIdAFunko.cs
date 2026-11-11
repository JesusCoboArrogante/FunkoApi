using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunkoAppi01.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProveedorIdAFunko : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProveedorId",
                table: "funkos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "funkos");
        }
    }
}
