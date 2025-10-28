using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Infra.Migrations.Data
{
    /// <inheritdoc />
    public partial class OrdersChangeStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reservation_status",
                schema: "orders",
                table: "order_items",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "reservation_status",
                schema: "orders",
                table: "order_items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
