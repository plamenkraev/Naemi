using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaemZZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLandlordCompanyDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAEMODATELI_FIRM_FIRM_ID",
                table: "NAEMODATELI");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "NAEMODATELI",
                type: "TEXT",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EIK",
                table: "NAEMODATELI",
                type: "TEXT",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompany",
                table: "NAEMODATELI",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MOL",
                table: "NAEMODATELI",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NAEMODATELI_FIRM_FIRM_ID",
                table: "NAEMODATELI",
                column: "FIRM_ID",
                principalTable: "FIRM",
                principalColumn: "firmID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAEMODATELI_FIRM_FIRM_ID",
                table: "NAEMODATELI");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "NAEMODATELI");

            migrationBuilder.DropColumn(
                name: "EIK",
                table: "NAEMODATELI");

            migrationBuilder.DropColumn(
                name: "IsCompany",
                table: "NAEMODATELI");

            migrationBuilder.DropColumn(
                name: "MOL",
                table: "NAEMODATELI");

            migrationBuilder.AddForeignKey(
                name: "FK_NAEMODATELI_FIRM_FIRM_ID",
                table: "NAEMODATELI",
                column: "FIRM_ID",
                principalTable: "FIRM",
                principalColumn: "firmID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
