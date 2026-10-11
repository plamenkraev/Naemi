using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaemZZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORI",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NAME = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORI", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EK_OBL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OBLAST = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    EKATTE = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    NAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EK_OBL", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FIRM",
                columns: table => new
                {
                    firmID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    firmNAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    firmADDRESS = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BULSTAT = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    firmMOL = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    firmEGN = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    firmLK = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    firmLKDATE = table.Column<DateTime>(type: "TEXT", nullable: true),
                    firmMVR = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIRM", x => x.firmID);
                });

            migrationBuilder.CreateTable(
                name: "EK_OBST",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EKATTE = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    NAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OBL_ID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EK_OBST", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EK_OBST_EK_OBL_OBL_ID",
                        column: x => x.OBL_ID,
                        principalTable: "EK_OBL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EKATTE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EKATTE = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true),
                    T_V_M = table.Column<string>(type: "TEXT", maxLength: 4, nullable: true),
                    NAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OBST_ID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EKATTE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EKATTE_EK_OBST_OBST_ID",
                        column: x => x.OBST_ID,
                        principalTable: "EK_OBST",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NAEMODATELI",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SURNAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LASTNAME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CITY = table.Column<int>(type: "INTEGER", nullable: true),
                    STREET = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EGN = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    LK = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    LKDATE = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MVR = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    FIRM_ID = table.Column<int>(type: "INTEGER", nullable: true),
                    OBSTINA = table.Column<int>(type: "INTEGER", nullable: true),
                    OBLAST = table.Column<int>(type: "INTEGER", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAEMODATELI", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAEMODATELI_EKATTE_CITY",
                        column: x => x.CITY,
                        principalTable: "EKATTE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NAEMODATELI_EK_OBL_OBLAST",
                        column: x => x.OBLAST,
                        principalTable: "EK_OBL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NAEMODATELI_EK_OBST_OBSTINA",
                        column: x => x.OBSTINA,
                        principalTable: "EK_OBST",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NAEMODATELI_FIRM_FIRM_ID",
                        column: x => x.FIRM_ID,
                        principalTable: "FIRM",
                        principalColumn: "firmID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IMOTI",
                columns: table => new
                {
                    imotiID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EKATTE_CITY = table.Column<int>(type: "INTEGER", nullable: true),
                    EKATTE_MUNICIPALITY = table.Column<int>(type: "INTEGER", nullable: true),
                    EKATTE_AREA = table.Column<int>(type: "INTEGER", nullable: true),
                    PLOST = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    CATEGORY = table.Column<int>(type: "INTEGER", nullable: true),
                    TERRAIN = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NOMER = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    LIMITS = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    nID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMOTI", x => x.imotiID);
                    table.ForeignKey(
                        name: "FK_IMOTI_CATEGORI_CATEGORY",
                        column: x => x.CATEGORY,
                        principalTable: "CATEGORI",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IMOTI_EKATTE_EKATTE_CITY",
                        column: x => x.EKATTE_CITY,
                        principalTable: "EKATTE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IMOTI_EK_OBL_EKATTE_AREA",
                        column: x => x.EKATTE_AREA,
                        principalTable: "EK_OBL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IMOTI_EK_OBST_EKATTE_MUNICIPALITY",
                        column: x => x.EKATTE_MUNICIPALITY,
                        principalTable: "EK_OBST",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IMOTI_NAEMODATELI_nID",
                        column: x => x.nID,
                        principalTable: "NAEMODATELI",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EK_OBST_OBL_ID",
                table: "EK_OBST",
                column: "OBL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_EKATTE_OBST_ID",
                table: "EKATTE",
                column: "OBST_ID");

            migrationBuilder.CreateIndex(
                name: "IX_IMOTI_CATEGORY",
                table: "IMOTI",
                column: "CATEGORY");

            migrationBuilder.CreateIndex(
                name: "IX_IMOTI_EKATTE_AREA",
                table: "IMOTI",
                column: "EKATTE_AREA");

            migrationBuilder.CreateIndex(
                name: "IX_IMOTI_EKATTE_CITY",
                table: "IMOTI",
                column: "EKATTE_CITY");

            migrationBuilder.CreateIndex(
                name: "IX_IMOTI_EKATTE_MUNICIPALITY",
                table: "IMOTI",
                column: "EKATTE_MUNICIPALITY");

            migrationBuilder.CreateIndex(
                name: "IX_IMOTI_nID",
                table: "IMOTI",
                column: "nID");

            migrationBuilder.CreateIndex(
                name: "IX_NAEMODATELI_CITY",
                table: "NAEMODATELI",
                column: "CITY");

            migrationBuilder.CreateIndex(
                name: "IX_NAEMODATELI_FIRM_ID",
                table: "NAEMODATELI",
                column: "FIRM_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAEMODATELI_OBLAST",
                table: "NAEMODATELI",
                column: "OBLAST");

            migrationBuilder.CreateIndex(
                name: "IX_NAEMODATELI_OBSTINA",
                table: "NAEMODATELI",
                column: "OBSTINA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IMOTI");

            migrationBuilder.DropTable(
                name: "CATEGORI");

            migrationBuilder.DropTable(
                name: "NAEMODATELI");

            migrationBuilder.DropTable(
                name: "EKATTE");

            migrationBuilder.DropTable(
                name: "FIRM");

            migrationBuilder.DropTable(
                name: "EK_OBST");

            migrationBuilder.DropTable(
                name: "EK_OBL");
        }
    }
}
