using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FakeWebShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Naam = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Naam = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beschrijving = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prijs = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Discriminator = table.Column<string>(type: "varchar(21)", maxLength: 21, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Maat = table.Column<int>(type: "int", nullable: true),
                    Stof = table.Column<int>(type: "int", nullable: true),
                    ClothingProduct_Kleur = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HasZipper = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    PocketType = table.Column<int>(type: "int", nullable: true),
                    PrintType = table.Column<int>(type: "int", nullable: true),
                    Inhoud = table.Column<int>(type: "int", nullable: true),
                    DrinkProduct_Kleur = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsThermisch = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Materiaal = table.Column<int>(type: "int", nullable: true),
                    VaatwasserBestendig = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Formaat = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoteBook_Kleur = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kleur = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CapaciteitmAh = table.Column<int>(type: "int", nullable: true),
                    InputType = table.Column<int>(type: "int", nullable: true),
                    OutputType = table.Column<int>(type: "int", nullable: true),
                    OndersteuntFastCharging = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    PortTotaal = table.Column<int>(type: "int", nullable: true),
                    Breedte = table.Column<double>(type: "double", nullable: true),
                    Hoogte = table.Column<double>(type: "double", nullable: true),
                    ToteBag_Breedte = table.Column<double>(type: "double", nullable: true),
                    ToteBag_Hoogte = table.Column<double>(type: "double", nullable: true),
                    ToteBag_Kleur = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
