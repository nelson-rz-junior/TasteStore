using Microsoft.EntityFrameworkCore.Migrations;

namespace TasteStore.DataAccess.Migrations
{
    public partial class AddBackgroundColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "Categories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_MenuItemId",
                table: "ShoppingCarts",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_MenuItems_MenuItemId",
                table: "ShoppingCarts",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_MenuItems_MenuItemId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_MenuItemId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "Categories");
        }
    }
}
