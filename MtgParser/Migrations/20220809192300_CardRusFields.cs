using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgParser.Migrations
{
    public partial class CardRusFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameRus",
                table: "Cards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TypeRus",
                table: "Cards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameRus",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "TypeRus",
                table: "Cards");
        }
    }
}
