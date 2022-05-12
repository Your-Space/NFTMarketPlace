using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NftProject.Data.Migrations
{
    public partial class AddAuctionInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionSaleModel",
                table: "AuctionSaleModel");

            migrationBuilder.RenameTable(
                name: "AuctionSaleModel",
                newName: "AuctionSales");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionSales",
                table: "AuctionSales",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuctionInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TokenId = table.Column<string>(type: "TEXT", nullable: false),
                    MinimalBid = table.Column<string>(type: "TEXT", nullable: false),
                    MinimalBidStep = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuctionSales",
                table: "AuctionSales");

            migrationBuilder.RenameTable(
                name: "AuctionSales",
                newName: "AuctionSaleModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuctionSaleModel",
                table: "AuctionSaleModel",
                column: "Id");
        }
    }
}
