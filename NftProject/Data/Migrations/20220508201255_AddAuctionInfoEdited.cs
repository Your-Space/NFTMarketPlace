using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NftProject.Data.Migrations
{
    public partial class AddAuctionInfoEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinalDate",
                table: "AuctionInfo",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "AuctionInfo",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalDate",
                table: "AuctionInfo");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AuctionInfo");
        }
    }
}
