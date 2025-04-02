using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseContext.Data.Migrations
{
    /// <inheritdoc />
    public partial class InfrastructureUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinutesToBusStop",
                table: "InfrastructureInfos",
                newName: "NearestShopId");

            migrationBuilder.AddColumn<int>(
                name: "NearestKindergartenId",
                table: "InfrastructureInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NearestMetroId",
                table: "InfrastructureInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NearestPharmacyId",
                table: "InfrastructureInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NearestSchoolId",
                table: "InfrastructureInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Kindergarten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kindergarten", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pharmacy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pharmacy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureInfos_NearestKindergartenId",
                table: "InfrastructureInfos",
                column: "NearestKindergartenId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureInfos_NearestMetroId",
                table: "InfrastructureInfos",
                column: "NearestMetroId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureInfos_NearestPharmacyId",
                table: "InfrastructureInfos",
                column: "NearestPharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureInfos_NearestSchoolId",
                table: "InfrastructureInfos",
                column: "NearestSchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_InfrastructureInfos_NearestShopId",
                table: "InfrastructureInfos",
                column: "NearestShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_InfrastructureInfos_Kindergarten_NearestKindergartenId",
                table: "InfrastructureInfos",
                column: "NearestKindergartenId",
                principalTable: "Kindergarten",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InfrastructureInfos_Metro_NearestMetroId",
                table: "InfrastructureInfos",
                column: "NearestMetroId",
                principalTable: "Metro",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InfrastructureInfos_Pharmacy_NearestPharmacyId",
                table: "InfrastructureInfos",
                column: "NearestPharmacyId",
                principalTable: "Pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InfrastructureInfos_School_NearestSchoolId",
                table: "InfrastructureInfos",
                column: "NearestSchoolId",
                principalTable: "School",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_InfrastructureInfos_Shop_NearestShopId",
                table: "InfrastructureInfos",
                column: "NearestShopId",
                principalTable: "Shop",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfrastructureInfos_Kindergarten_NearestKindergartenId",
                table: "InfrastructureInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_InfrastructureInfos_Metro_NearestMetroId",
                table: "InfrastructureInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_InfrastructureInfos_Pharmacy_NearestPharmacyId",
                table: "InfrastructureInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_InfrastructureInfos_School_NearestSchoolId",
                table: "InfrastructureInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_InfrastructureInfos_Shop_NearestShopId",
                table: "InfrastructureInfos");

            migrationBuilder.DropTable(
                name: "Kindergarten");

            migrationBuilder.DropTable(
                name: "Metro");

            migrationBuilder.DropTable(
                name: "Pharmacy");

            migrationBuilder.DropTable(
                name: "School");

            migrationBuilder.DropTable(
                name: "Shop");

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureInfos_NearestKindergartenId",
                table: "InfrastructureInfos");

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureInfos_NearestMetroId",
                table: "InfrastructureInfos");

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureInfos_NearestPharmacyId",
                table: "InfrastructureInfos");

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureInfos_NearestSchoolId",
                table: "InfrastructureInfos");

            migrationBuilder.DropIndex(
                name: "IX_InfrastructureInfos_NearestShopId",
                table: "InfrastructureInfos");

            migrationBuilder.DropColumn(
                name: "NearestKindergartenId",
                table: "InfrastructureInfos");

            migrationBuilder.DropColumn(
                name: "NearestMetroId",
                table: "InfrastructureInfos");

            migrationBuilder.DropColumn(
                name: "NearestPharmacyId",
                table: "InfrastructureInfos");

            migrationBuilder.DropColumn(
                name: "NearestSchoolId",
                table: "InfrastructureInfos");

            migrationBuilder.RenameColumn(
                name: "NearestShopId",
                table: "InfrastructureInfos",
                newName: "MinutesToBusStop");
        }
    }
}
