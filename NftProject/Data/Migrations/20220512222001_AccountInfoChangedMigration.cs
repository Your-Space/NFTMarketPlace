using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NftProject.Data.Migrations
{
    public partial class AccountInfoChangedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimalBid",
                table: "AuctionInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MinimalBid",
                table: "AuctionInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
