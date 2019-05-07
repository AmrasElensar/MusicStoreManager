using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicStoreManager.Migrations
{
    public partial class AlbumOfTheWeek : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAlbumOfTheWeek",
                table: "Album",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlbumOfTheWeek",
                table: "Album");
        }
    }
}
