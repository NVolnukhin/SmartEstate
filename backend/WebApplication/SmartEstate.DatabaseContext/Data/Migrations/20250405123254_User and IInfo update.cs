using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DatabaseContext.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserandIInfoupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                   
            // Удаляем связи, чтобы можно было изменить типы
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_Users_UserId",
                table: "UserFavorites");
            
            migrationBuilder.DropForeignKey(
                name: "FK_UserComparisons_Users_UserId",
                table: "UserComparisons");

            // Удаляем индексы
            migrationBuilder.DropIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites");
            
            migrationBuilder.DropIndex(
                name: "IX_UserComparisons_UserId",
                table: "UserComparisons");

            // Создаем временные колонки
            migrationBuilder.AddColumn<Guid>(
                name: "NewUserId",
                table: "UserFavorites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            
            migrationBuilder.AddColumn<Guid>(
                name: "NewUserId",
                table: "UserComparisons",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            

            // Удаляем старые колонки
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserFavorites");
            
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserComparisons");

            // Переименовываем новые колонки
            migrationBuilder.RenameColumn(
                name: "NewUserId",
                table: "UserFavorites",
                newName: "UserId");
            
            migrationBuilder.RenameColumn(
                name: "NewUserId",
                table: "UserComparisons",
                newName: "UserId");

            // Удаляем старую таблицу Users
            migrationBuilder.DropTable(
                name: "Users");

            // Создаем новую таблицу Users
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            // Восстанавливаем индексы и связи
            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserComparisons_UserId",
                table: "UserComparisons",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_Users_UserId",
                table: "UserFavorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserComparisons_Users_UserId",
                table: "UserComparisons",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            // Удаляем старый столбец
            migrationBuilder.DropColumn(
                name: "Facilities",
                table: "InfrastructureInfos");
        }
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserFavorites",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserComparisons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Facilities",
                table: "InfrastructureInfos",
                type: "text",
                nullable: true);
        }
    }
}
