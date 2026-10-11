using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaemZZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractTemplateId",
                table: "FIRM",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContractTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Html = table.Column<string>(type: "TEXT", nullable: false),
                    LegacyRtfFile = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FIRM_ContractTemplateId",
                table: "FIRM",
                column: "ContractTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FIRM_ContractTemplates_ContractTemplateId",
                table: "FIRM",
                column: "ContractTemplateId",
                principalTable: "ContractTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FIRM_ContractTemplates_ContractTemplateId",
                table: "FIRM");

            migrationBuilder.DropTable(
                name: "ContractTemplates");

            migrationBuilder.DropIndex(
                name: "IX_FIRM_ContractTemplateId",
                table: "FIRM");

            migrationBuilder.DropColumn(
                name: "ContractTemplateId",
                table: "FIRM");
        }
    }
}
