using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseContext.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedrescompl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "Flats",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResidentialComplex",
                table: "Buildings");

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "Flats",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
