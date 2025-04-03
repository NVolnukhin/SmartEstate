using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseContext.Data.Migrations
{
    /// <inheritdoc />
    public partial class BuildingCoordintesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Buildings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Buildings",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Buildings");
        }
    }
}
