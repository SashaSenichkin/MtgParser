using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgParser.Migrations
{
    public partial class addField_boolField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CardsNames",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRus",
                table: "Cards",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CardsNames");

            migrationBuilder.AlterColumn<byte>(
                name: "IsRus",
                table: "Cards",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
