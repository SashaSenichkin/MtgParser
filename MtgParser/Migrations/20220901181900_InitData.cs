using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgParser.Migrations
{
    public partial class InitData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Keywords",
                columns: new[] { "Id", "Name", "RusName" },
                values: new object[,]
                {
                    { 1, "Deathtouch", "Смертельное касание" },
                    { 2, "Defender", "Защитник" },
                    { 3, "Double strike", "Двойной удар" },
                    { 4, "Enchant", "Зачаровать" },
                    { 5, "Equip", "Снарядить" },
                    { 6, "First strike", "Первый удар" },
                    { 7, "Flash", "Миг" },
                    { 8, "Flying", "Полет" },
                    { 9, "Haste", "Ускорение" },
                    { 10, "Hexproof", "Порчеустойчивость" },
                    { 11, "Indestructible", "Неразрушимость" },
                    { 12, "Lifelink", "Цепь жизни" },
                    { 13, "Menace", "Угроза" },
                    { 14, "Protection", "Защита" },
                    { 15, "Prowess", "Искусность" },
                    { 16, "Reach", "Захват" },
                    { 17, "Trample", "Пробивной удар" },
                    { 18, "Vigilance", "Бдительность" }
                });

            migrationBuilder.InsertData(
                table: "Rarities",
                columns: new[] { "Id", "Name", "RusName" },
                values: new object[,]
                {
                    { 1, "Common", "Обычная" },
                    { 2, "Uncommon", "Необычная" },
                    { 3, "Rare", "Редкая" },
                    { 4, "Mythic", "Раритетная" },
                    { 5, "Special", "Специальная" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Keywords",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
