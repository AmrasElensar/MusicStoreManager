using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicStoreManager.Migrations
{
    public partial class CascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Album__ArtistId__276EDEB3",
                table: "Album");

            migrationBuilder.AddForeignKey(
                name: "FK__Album__ArtistId__276EDEB3",
                table: "Album",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "ArtistId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Album__ArtistId__276EDEB3",
                table: "Album");

            migrationBuilder.AddForeignKey(
                name: "FK__Album__ArtistId__276EDEB3",
                table: "Album",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "ArtistId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
