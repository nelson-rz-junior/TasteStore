using Microsoft.EntityFrameworkCore.Migrations;

namespace TasteStore.DataAccess.Migrations
{
    public partial class AddPaymentMethodType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodTypes",
                table: "OrderHeaders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethodTypes",
                table: "OrderHeaders");
        }
    }
}
