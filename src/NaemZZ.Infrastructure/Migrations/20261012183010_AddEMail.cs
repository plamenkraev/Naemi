using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaemZZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEMail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NAEMODATELI",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "NAEMODATELI");
        }
    }
}
