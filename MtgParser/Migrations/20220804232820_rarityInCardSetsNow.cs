using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MtgParser.Migrations
{
    public partial class rarityInCardSetsNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Rarities_RarityId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_RarityId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "RarityId",
                table: "Cards");

            migrationBuilder.AddColumn<int>(
                name: "RarityId",
                table: "CardsSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CardsSets_RarityId",
                table: "CardsSets",
                column: "RarityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardsSets_Rarities_RarityId",
                table: "CardsSets",
                column: "RarityId",
                principalTable: "Rarities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardsSets_Rarities_RarityId",
                table: "CardsSets");

            migrationBuilder.DropIndex(
                name: "IX_CardsSets_RarityId",
                table: "CardsSets");

            migrationBuilder.DropColumn(
                name: "RarityId",
                table: "CardsSets");

            migrationBuilder.AddColumn<int>(
                name: "RarityId",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_RarityId",
                table: "Cards",
                column: "RarityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Rarities_RarityId",
                table: "Cards",
                column: "RarityId",
                principalTable: "Rarities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
