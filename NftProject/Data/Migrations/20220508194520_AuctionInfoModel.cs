using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NftProject.Data.Migrations
{
    public partial class AuctionInfoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionSaleModels",
                table: "AuctionSaleModels");

            migrationBuilder.RenameTable(
                name: "AuctionSaleModels",
                newName: "AuctionSaleModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionSaleModel",
                table: "AuctionSaleModel",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionSaleModel",
                table: "AuctionSaleModel");

            migrationBuilder.RenameTable(
                name: "AuctionSaleModel",
                newName: "AuctionSaleModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionSaleModels",
                table: "AuctionSaleModels",
                column: "Id");
        }
    }
}
