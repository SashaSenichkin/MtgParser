using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MtgParser.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameRus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cmc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeRus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextRus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Power = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Toughness = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardsNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameRus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsFoil = table.Column<bool>(type: "bit", nullable: false),
                    SetShort = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardsNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rarities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rarities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SetImg = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardKeyword",
                columns: table => new
                {
                    CardsId = table.Column<int>(type: "int", nullable: false),
                    KeywordsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardKeyword", x => new { x.CardsId, x.KeywordsId });
                    table.ForeignKey(
                        name: "FK_CardKeyword_Cards_CardsId",
                        column: x => x.CardsId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardKeyword_Keywords_KeywordsId",
                        column: x => x.KeywordsId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardsSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsFoil = table.Column<byte>(type: "tinyint", nullable: false),
                    RarityId = table.Column<int>(type: "int", nullable: false),
                    ManualPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardsSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardsSets_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardsSets_Rarities_RarityId",
                        column: x => x.RarityId,
                        principalTable: "Rarities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardsSets_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CardSetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prices_CardsSets_CardSetId",
                        column: x => x.CardSetId,
                        principalTable: "CardsSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CardKeyword_KeywordsId",
                table: "CardKeyword",
                column: "KeywordsId");

            migrationBuilder.CreateIndex(
                name: "IX_CardsSets_CardId",
                table: "CardsSets",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardsSets_RarityId",
                table: "CardsSets",
                column: "RarityId");

            migrationBuilder.CreateIndex(
                name: "IX_CardsSets_SetId",
                table: "CardsSets",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_CardSetId",
                table: "Prices",
                column: "CardSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardKeyword");

            migrationBuilder.DropTable(
                name: "CardsNames");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "CardsSets");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Rarities");

            migrationBuilder.DropTable(
                name: "Sets");
        }
    }
}
