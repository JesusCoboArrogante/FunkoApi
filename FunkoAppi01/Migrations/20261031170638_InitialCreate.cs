using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunkoAppi01.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "funkos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    stock = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoriaId = table.Column<long>(type: "INTEGER", nullable: false),
                    is_deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_funkos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_funkos_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categorias_nombre",
                table: "categorias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_funkos_CategoriaId",
                table: "funkos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_funkos_nombre",
                table: "funkos",
                column: "nombre",
                unique: true,
                filter: "\"is_deleted\" = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "funkos");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
