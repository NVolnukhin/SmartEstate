using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseContext.Data.Migrations
{
    /// <inheritdoc />
    public partial class InfrastructureInfoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_InfrastructureInfos_InfrastructureInfoId",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_InfrastructureInfoId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "InfrastructureInfoId",
                table: "Buildings");

            migrationBuilder.AlterColumn<string>(
                name: "Facilities",
                table: "InfrastructureInfos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "InfrastructureInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureInfos_BuildingId",
                table: "InfrastructureInfos",
                column: "BuildingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InfrastructureInfos_Buildings_BuildingId",
                table: "InfrastructureInfos",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "BuildingId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfrastructureInfos_Buildings_BuildingId",
                table: "InfrastructureInfos");

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureInfos_BuildingId",
                table: "InfrastructureInfos");

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "InfrastructureInfos");

            migrationBuilder.AlterColumn<string>(
                name: "Facilities",
                table: "InfrastructureInfos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InfrastructureInfoId",
                table: "Buildings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_InfrastructureInfoId",
                table: "Buildings",
                column: "InfrastructureInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_InfrastructureInfos_InfrastructureInfoId",
                table: "Buildings",
                column: "InfrastructureInfoId",
                principalTable: "InfrastructureInfos",
                principalColumn: "InfrastructureInfoId");
        }
    }
}
