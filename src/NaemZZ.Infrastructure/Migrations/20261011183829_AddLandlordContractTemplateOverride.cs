using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaemZZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLandlordContractTemplateOverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractTemplateId",
                table: "NAEMODATELI",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NAEMODATELI_ContractTemplateId",
                table: "NAEMODATELI",
                column: "ContractTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_NAEMODATELI_ContractTemplates_ContractTemplateId",
                table: "NAEMODATELI",
                column: "ContractTemplateId",
                principalTable: "ContractTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NAEMODATELI_ContractTemplates_ContractTemplateId",
                table: "NAEMODATELI");

            migrationBuilder.DropIndex(
                name: "IX_NAEMODATELI_ContractTemplateId",
                table: "NAEMODATELI");

            migrationBuilder.DropColumn(
                name: "ContractTemplateId",
                table: "NAEMODATELI");
        }
    }
}
