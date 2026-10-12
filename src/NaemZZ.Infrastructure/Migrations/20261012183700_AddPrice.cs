using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaemZZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "NAEMODATELI",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "FIRM",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "NAEMODATELI");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "FIRM");
        }
    }
}
