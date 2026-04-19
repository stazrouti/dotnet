using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bibliotheque.Migrations
{
    /// <inheritdoc />
    public partial class AddAvisNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "avis_cin_numinv_unique",
                table: "avis");

            migrationBuilder.AlterColumn<string>(
                name: "numinv",
                table: "avis",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "commentaire",
                table: "avis",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                defaultValueSql: "(NULL)",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldDefaultValueSql: "(NULL)");

            migrationBuilder.AlterColumn<string>(
                name: "cin",
                table: "avis",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "avis_cin_numinv_unique",
                table: "avis",
                columns: new[] { "cin", "numinv" },
                unique: true,
                filter: "[cin] IS NOT NULL AND [numinv] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "avis_cin_numinv_unique",
                table: "avis");

            migrationBuilder.AlterColumn<string>(
                name: "numinv",
                table: "avis",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "commentaire",
                table: "avis",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValueSql: "(NULL)",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldDefaultValueSql: "(NULL)");

            migrationBuilder.AlterColumn<string>(
                name: "cin",
                table: "avis",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "avis_cin_numinv_unique",
                table: "avis",
                columns: new[] { "cin", "numinv" },
                unique: true);
        }
    }
}
