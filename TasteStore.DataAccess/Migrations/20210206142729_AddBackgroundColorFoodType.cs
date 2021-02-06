using Microsoft.EntityFrameworkCore.Migrations;

namespace TasteStore.DataAccess.Migrations
{
    public partial class AddBackgroundColorFoodType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "FoodTypes",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "FoodTypes");
        }
    }
}
