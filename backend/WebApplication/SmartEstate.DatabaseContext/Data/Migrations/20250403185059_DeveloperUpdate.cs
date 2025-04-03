using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseContext.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeveloperUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BuildingsCount",
                table: "Developers",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BuildingsCount",
                table: "Developers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
