using Microsoft.EntityFrameworkCore.Migrations;

namespace TasteStore.DataAccess.Migrations
{
    public partial class AddPaymentStatusCheckout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CheckoutPaymentStatus",
                table: "OrderHeaders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutPaymentStatus",
                table: "OrderHeaders");
        }
    }
}
