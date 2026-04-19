using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bibliotheque.Migrations
{
    /// <inheritdoc />
    public partial class CreateAvisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "avis",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    numinv = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    note = table.Column<int>(type: "int", nullable: false),
                    commentaire = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, defaultValueSql: "(NULL)"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_avis_id", x => x.id);
                    table.ForeignKey(
                        name: "avis$avis_cin_foreign",
                        column: x => x.cin,
                        principalTable: "etudiants",
                        principalColumn: "cin",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "avis$avis_numinv_foreign",
                        column: x => x.numinv,
                        principalTable: "livres",
                        principalColumn: "numinventaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "avis_cin_foreign",
                table: "avis",
                column: "cin");

            migrationBuilder.CreateIndex(
                name: "avis_cin_numinv_unique",
                table: "avis",
                columns: new[] { "cin", "numinv" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "avis_numinv_foreign",
                table: "avis",
                column: "numinv");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "avis");
        }
    }
}
