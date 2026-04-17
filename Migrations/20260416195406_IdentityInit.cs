using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bibliotheque.Migrations
{
    /// <inheritdoc />
    public partial class IdentityInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NomComplet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "etudiants",
                columns: table => new
                {
                    cin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    matricule = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    nom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    prenom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    Niveau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    filiere = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    tel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    rfid = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    qr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    commentaire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    observation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etudiants_cin", x => x.cin);
                });

            migrationBuilder.CreateTable(
                name: "livres",
                columns: table => new
                {
                    numinventaire = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    isbn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    titre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    auteur = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    editeur = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    datepublication = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(NULL)"),
                    datearrivage = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(NULL)"),
                    résumé = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    cote = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_livres_numinventaire", x => x.numinventaire);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visites",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CIN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValueSql: "(NULL)"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visites_id", x => x.id);
                    table.ForeignKey(
                        name: "visites$visites_cin_foreign",
                        column: x => x.CIN,
                        principalTable: "etudiants",
                        principalColumn: "cin",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "emprunts",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    numinv = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    dateemprunt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    dateretour = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    estretour = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_emprunts_id", x => x.id);
                    table.ForeignKey(
                        name: "emprunts$emprunts_cin_foreign",
                        column: x => x.cin,
                        principalTable: "etudiants",
                        principalColumn: "cin",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "emprunts$emprunts_numinv_foreign",
                        column: x => x.numinv,
                        principalTable: "livres",
                        principalColumn: "numinventaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "motcles",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numinv = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    mot = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(NULL)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motcles_id", x => x.id);
                    table.ForeignKey(
                        name: "motcles$motcles_numinv_foreign",
                        column: x => x.numinv,
                        principalTable: "livres",
                        principalColumn: "numinventaire",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "emprunts_cin_foreign",
                table: "emprunts",
                column: "cin");

            migrationBuilder.CreateIndex(
                name: "emprunts_numinv_foreign",
                table: "emprunts",
                column: "numinv");

            migrationBuilder.CreateIndex(
                name: "etudiants$etudiants_email_unique",
                table: "etudiants",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "etudiants$etudiants_matricule_unique",
                table: "etudiants",
                column: "matricule",
                unique: true,
                filter: "[matricule] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "etudiants$etudiants_qr_unique",
                table: "etudiants",
                column: "qr",
                unique: true,
                filter: "[qr] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "etudiants$etudiants_rfid_unique",
                table: "etudiants",
                column: "rfid",
                unique: true,
                filter: "[rfid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "motcles_numinv_foreign",
                table: "motcles",
                column: "numinv");

            migrationBuilder.CreateIndex(
                name: "visites_cin_foreign",
                table: "visites",
                column: "CIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "emprunts");

            migrationBuilder.DropTable(
                name: "motcles");

            migrationBuilder.DropTable(
                name: "visites");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "livres");

            migrationBuilder.DropTable(
                name: "etudiants");
        }
    }
}
